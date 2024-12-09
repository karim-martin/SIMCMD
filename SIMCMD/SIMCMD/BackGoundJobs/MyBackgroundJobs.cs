using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SIMCMD.Core;
using FluentFTP;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SIMCMD.Data;
using System.Linq;
using SIMCMD.Models;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace SIMCMD.BackGroundJobs
{
    public class MyBackgroundJobs : IMyBackgroundJobs
    {
        private readonly ILogger<MyBackgroundJobs> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public MyBackgroundJobs(ILogger<MyBackgroundJobs> logger, IWebHostEnvironment env, IConfiguration configuration, ApplicationDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public void DownloadFiles()
        {
            _logger.LogInformation("DownloadFiles method start at: {time}", DateTimeOffset.Now);

            string sFTPFolder = _configuration.GetSection("FolderConfig:DownloadFiles:SFTPFolder").Value;
            string sFTPCopyFolder = _configuration.GetSection("FolderConfig:DownloadFiles:SFTPCopyFolder").Value;
            string fileTranslatorInputFolder = _configuration.GetSection("FolderConfig:DownloadFiles:FileTranslatorInputFolder").Value;
            string fileTranslatorOutputFolder = _configuration.GetSection("FolderConfig:DownloadFiles:FileTranslatorOutputFolder").Value;
            string fileTranslatorPath = _configuration.GetSection("FolderConfig:DownloadFiles:FileTranslatorPath").Value;
            string operationOption = _configuration.GetSection("FolderConfig:DownloadFiles:OperationOption").Value;

            var providers = _context.Provider.ToList();

            foreach (var provider in providers)
            {
                string sourcePath = Path.Combine(sFTPFolder, provider.FolderName);

                if (!Directory.Exists(sourcePath))
                {
                    _logger.LogWarning("Source directory does not exist: {sourcePath}", sourcePath);
                    continue;
                }

                foreach (var filePath in Directory.GetFiles(sourcePath))
                {
                    string fileName = Path.GetFileName(filePath);

                    if (provider.Unwrap)
                    {
                        string inputDirectory = Path.Combine(fileTranslatorInputFolder, provider.FolderName);
                        string outputDirectory = Path.Combine(fileTranslatorOutputFolder, provider.FolderName);
                        string fileForProcessing = Path.Combine(inputDirectory, fileName);

                        Directory.CreateDirectory(inputDirectory);

                        File.Move(filePath, fileForProcessing, true); // Overwrite if exists

                        string appSettingsPath = Path.Combine(fileTranslatorPath, "appsettings.json");

                        string newAppSettingsJson = $@"
                        {{
                            ""InputDirectory"": ""F:\\Bulk Claim Upload\\Files_Convert\\{provider.FolderName}"",
                            ""OutputDirectory"": ""F:\\Bulk Claim Upload\\Files_Converted\\{provider.FolderName}"",
                            ""FilesToFormatPrefixes"": [ ""{provider.ProviderCode}_"" ],
                            ""EnableOverwrite"":  true
                        }}";

                        File.WriteAllText(appSettingsPath, newAppSettingsJson);

                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = Path.Combine(fileTranslatorPath, "CaymanFirst.FileTranslator.exe"),
                                Arguments = "",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true,
                                WorkingDirectory = fileTranslatorPath
                            }
                        };

                        process.Start();
                        string output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        _logger.LogInformation("CaymanFirst.FileTranslator.exe completed with output: {output}", output);

                        if (Directory.Exists(outputDirectory))
                        {
                            string destinationPathForUnwrapped = Path.Combine(sFTPCopyFolder, provider.FolderName);
                            Directory.CreateDirectory(destinationPathForUnwrapped);

                            foreach (var unwrappedFilePath in Directory.GetFiles(outputDirectory))
                            {
                                string unwrappedFileName = Path.GetFileName(unwrappedFilePath);
                                string destFile = Path.Combine(destinationPathForUnwrapped, unwrappedFileName);
                                File.Move(unwrappedFilePath, destFile, true);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Output directory for unwrapped files does not exist: {outputDirectory}", outputDirectory);
                        }

                        _logger.LogInformation("CaymanFirst.FileTranslator.exe completed with output: {output}", output);
                    }
                    else
                    {
                        string destinationPath = Path.Combine(sFTPCopyFolder, provider.FolderName);
                        string destFile = Path.Combine(destinationPath, fileName);

                        Directory.CreateDirectory(destinationPath);

                        if (operationOption.Equals("Copy", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Copy(filePath, destFile, true);
                        }
                        else if (operationOption.Equals("Move", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Move(filePath, destFile, true);
                        }
                    }
                }
            }

            _logger.LogInformation("DownloadFiles method stop at: {time}", DateTimeOffset.Now);
        }

        public async Task ConvertFilesAsync()
        {
            _logger.LogInformation("ConvertFiles method start at: {time}", DateTimeOffset.Now);
            string output = "";
            string sourceFolder = _configuration.GetSection("FolderConfig:DownloadFiles:SourceFolder").Value;
            string executablePath = _configuration.GetSection("FolderConfig:DownloadFiles:ExecutablePath").Value;

            var files = Directory.GetFiles(sourceFolder);

            foreach (var file in files)
            {
                string extension = Path.GetExtension(file).ToLower();
                string fileName = Path.GetFileName(file);
                string processName = null;

                // Determine the process to use based on the file extension or name
                if (extension == ".csv" || extension == ".txt" || extension == ".xlsx" || extension == ".xls")
                {
                    processName = "RXFLATFILE_TO_837P_47.exe";
                }
                else if (fileName.Contains("837i", StringComparison.OrdinalIgnoreCase))
                {
                    processName = "CONVERT_837I_TO_837P.exe";
                }

                if (!string.IsNullOrEmpty(processName))
                {
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = Path.Combine(executablePath, processName),
                        Arguments = $"\"{file}\"", // Pass the file as an argument to the executable
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        WorkingDirectory = executablePath // Set working directory if needed by the executable
                    };

                    using (var process = Process.Start(processStartInfo))
                    {
                        await process.WaitForExitAsync();
                        // Optionally, read the output
                        output = await process.StandardOutput.ReadToEndAsync();
                        _logger.LogInformation($"{processName} completed for {fileName}: {output}");
                    }

                    // Add conversion data to _context.FileConversion
                    var conversionRecord = new FileConversion
                    {
                        FileName = fileName,
                        FileLog = output
                    };

                    _context.FileConversion.Add(conversionRecord);
                    await _context.SaveChangesAsync();
                }
            }

            _logger.LogInformation("ConvertFiles method stop at: {time}", DateTimeOffset.Now);
        }


        public void ImportFiles()
        {
            _logger.LogInformation("Local copy or move method two start at: {time}", DateTimeOffset.Now);

            string convert_837I_Folder = _configuration.GetSection("FolderConfig:ImportFiles:CONVERT_837I_TO_837P_Folder").Value;
            string convert_RXFLATFILE_Folder = _configuration.GetSection("FolderConfig:ImportFiles:RXFLATFILE_TO_837P_47_Folder").Value;
            string qC_IncomingFolder = _configuration.GetSection("FolderConfig:ImportFiles:QC_IncomingFolder").Value;

            // Ensure the QC Incoming Folder exists
            Directory.CreateDirectory(qC_IncomingFolder);

            // Move files from convert_837I_Folder to qC_IncomingFolder
            MoveFiles(convert_837I_Folder, qC_IncomingFolder);

            // Move files from convert_RXFLATFILE_Folder to qC_IncomingFolder
            MoveFiles(convert_RXFLATFILE_Folder, qC_IncomingFolder);

            _logger.LogInformation("Local copy or move method two stop at: {time}", DateTimeOffset.Now);
        }

        private void MoveFiles(string sourceFolder, string destinationFolder)
        {
            var files = Directory.GetFiles(sourceFolder);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(destinationFolder, fileName);

                // Check if the file already exists in the destination folder to avoid overwriting
                if (!File.Exists(destFile))
                {
                    File.Move(file, destFile);
                }
                else
                {
                    _logger.LogWarning("File already exists and won't be moved: {fileName}", fileName);
                    // Optionally, handle the scenario where the file already exists in the destination folder,
                    // such as renaming the incoming file, overwriting, or logging a message.
                }
            }
        }
    }
}
