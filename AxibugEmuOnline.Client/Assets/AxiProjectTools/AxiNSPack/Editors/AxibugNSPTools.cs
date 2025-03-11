using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Text;

namespace AxibugEmuOnline.Editors
{
    public class AxibugNSPTools : Editor
    {
        static string WorkRoot = Path.GetFullPath(Path.Combine(Application.dataPath,"AxiProjectTools/AxiNSPack"));
        static string switch_keys = Path.GetFullPath(Path.Combine(Application.dataPath, "AxiProjectTools/AxiNSPack/switch_keys"));
        static string hacpack_root = Path.GetFullPath(Path.Combine(Application.dataPath, "AxiProjectTools/AxiNSPack/hacpack"));
        static Dictionary<string, string> tools = new Dictionary<string, string>();
        static string prodKeysPath;
        
        [MenuItem("Axibug��ֲ����/Switch/AxibugNSPTools/RepackNSP")]
        static void RepackNSP()
        {
            if (!CheckEnvironmentVariable())
                return;

            string path = EditorUtility.OpenFilePanel(
                title: "ѡ�� .nsp �ļ�",
                directory: Path.Combine(Application.dataPath,".."), // Ĭ��·��Ϊ��Ŀ Assets Ŀ¼
                extension: "nsp" // �����ļ�����Ϊ .nsp
            );

            if (string.IsNullOrEmpty(path))
                return;

            RepackNSP(path);
        }
        static bool CheckEnvironmentVariable()
        {
            // ��ȡ������������Ҫ��ӻ���������飩
            string sdkRoot = Environment.GetEnvironmentVariable("NINTENDO_SDK_ROOT");
            if (string.IsNullOrEmpty(sdkRoot))
            {
                Debug.LogError($"[AxibugNSPTools]������ȷ���û�������:NINTENDO_SDK_ROOT,(�������ã���֤���ú󳹵�����Unity Hub��Unity)");
                return false;
            }

            #region ��ȡprod.keys�ļ�·��
            prodKeysPath = Path.Combine(
                switch_keys,
                "prod.keys"
            );

            if (!File.Exists(prodKeysPath))
            {
                Debug.LogError($"[AxibugNSPTools]δ�ҵ� prod.keys! ����׼���ļ���path:{prodKeysPath}");
                return false;
            }
            #endregion

            return true;
        }

        static void RepackNSP(string nspFile)
        {
            #region ��ʼ������·��
            // ��ȡ������������Ҫ��ӻ���������飩
            string sdkRoot = Environment.GetEnvironmentVariable("NINTENDO_SDK_ROOT");
            tools["authoringTool"] = Path.Combine(sdkRoot, "Tools/CommandLineTools/AuthoringTool/AuthoringTool.exe");
            tools["hacPack"] = hacpack_root;
            #endregion

            #region ����NSP�ļ�·��
            string nspFilePath = nspFile;
            string nspFileName = Path.GetFileName(nspFilePath);
            string nspParentDir = Path.GetDirectoryName(nspFilePath);
            #endregion

            #region ��ȡTitle ID
            string titleID = ExtractTitleID(nspFilePath) ?? ManualTitleIDInput();
            Debug.Log($"[AxibugNSPTools]Using Title ID: {titleID}");
            #endregion

            #region ������ʱĿ¼
            CleanDirectory(Path.Combine(nspParentDir, "repacker_extract"));
            CleanDirectory(Path.Combine(Path.GetTempPath(), "NCA"));
            CleanDirectory(Path.Combine(WorkRoot, "hacpack_backup"));
            #endregion

            #region ���NSP�ļ�
            string extractPath = Path.Combine(nspParentDir, "repacker_extract");
            ExecuteCommand($"{tools["authoringTool"]} extract -o \"{extractPath}\" \"{nspFilePath}\"");

            var (controlPath, programPath) = FindNACPAndNPDPaths(extractPath);
            if (controlPath == null || programPath == null)
            {
                Debug.LogError("[AxibugNSPTools] Critical directory structure not found!");
                return;
            }
            #endregion

            #region �ؽ�NCA/NSP
            string tmpPath = Path.Combine(Path.GetTempPath(), "NCA");
            string programNCA = BuildProgramNCA(tmpPath, titleID, programPath);
            string controlNCA = BuildControlNCA(tmpPath, titleID, controlPath);
            BuildMetaNCA(tmpPath, titleID, programNCA, controlNCA);

            string outputNSP = BuildFinalNSP(nspFilePath, nspParentDir, tmpPath, titleID);
            Debug.Log($"[AxibugNSPTools]Repacking completed: {outputNSP}");
            #endregion
        }

        #region ��������
        static string GetUserInput()
        {
            Console.Write("Enter the NSP filepath: ");
            return Console.ReadLine();
        }

        static string ExtractTitleID(string path)
        {
            var match = Regex.Match(path, @"0100[\dA-Fa-f]{12}");
            return match.Success ? match.Value : null;
        }

        static string ManualTitleIDInput()
        {
            Console.Write("Enter Title ID manually: ");
            return Console.ReadLine().Trim();
        }

        static void CleanDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                while (Directory.Exists(path)) ; // �ȴ�ɾ�����
            }
        }

        static (string, string) FindNACPAndNPDPaths(string basePath)
        {
            foreach (var dir in Directory.GetDirectories(basePath))
            {
                if (File.Exists(Path.Combine(dir, "fs0/control.nacp")))
                    return (dir, null);
                if (File.Exists(Path.Combine(dir, "fs0/main.npdm")))
                    return (null, dir);
            }
            return (null, null);
        }

        static string ExecuteCommand(string command)
        {
            var process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,  // ���Ӵ������ض���
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,  // ��ȷָ������
                    StandardErrorEncoding = Encoding.UTF8
                }
            };

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            // ʹ���¼�������򲶻�ʵʱ���
            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    outputBuilder.AppendLine(args.Data);
                    Debug.Log($"[AxibugNSPTools]{args.Data}");
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    errorBuilder.AppendLine(args.Data);
                    Debug.LogError($"[AxibugNSPTools]{args.Data}");
                }
            };

            process.Start();

            // ��ʼ�첽��ȡ���
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // �ȴ������˳�����ʱ���ѹرգ�
            process.WaitForExit();

            // ��������Ϣ���ӵ������
            if (errorBuilder.Length > 0)
            {
                outputBuilder.AppendLine("\nError Output:");
                outputBuilder.Append(errorBuilder);
            }

            return outputBuilder.ToString();
        }
        #endregion

        #region NCA�����߼�
        static string BuildProgramNCA(string tmpPath, string titleID, string programDir)
        {
            string args = $"-k \"{prodKeysPath}\" -o \"{tmpPath}\" --titleid {titleID} " +
                          $"--type nca --ncatype program --exefsdir \"{programDir}/fs0\" " +
                          $"--romfsdir \"{programDir}/fs1\" --logodir \"{programDir}/fs2\"";

            string output = ExecuteCommand($"{tools["hacPack"]} {args}");
            return ParseNCAOutput(output, "Program");
        }

        static string BuildControlNCA(string tmpPath, string titleID, string controlDir)
        {
            string args = $"-k \"{prodKeysPath}\" -o \"{tmpPath}\" --titleid {titleID} " +
                          $"--type nca --ncatype control --romfsdir \"{controlDir}/fs0\"";

            string output = ExecuteCommand($"{tools["hacPack"]} {args}");
            return ParseNCAOutput(output, "Control");
        }

        static void BuildMetaNCA(string tmpPath, string titleID, string programNCA, string controlNCA)
        {
            string args = $"-k \"{prodKeysPath}\" -o \"{tmpPath}\" --titleid {titleID} " +
                          $"--type nca --ncatype meta --titletype application " +
                          $"--programnca \"{programNCA}\" --controlnca \"{controlNCA}\"";

            ExecuteCommand($"{tools["hacPack"]} {args}");
        }

        static string BuildFinalNSP(string origPath, string parentDir, string tmpPath, string titleID)
        {
            string outputPath = origPath.Replace(".nsp", "_repacked.nsp");
            if (File.Exists(outputPath)) File.Delete(outputPath);

            string args = $"-k \"{prodKeysPath}\" -o \"{parentDir}\" --titleid {titleID} " +
                          $"--type nsp --ncadir \"{tmpPath}\"";

            ExecuteCommand($"{tools["hacPack"]} {args}");
            File.Move(Path.Combine(parentDir, $"{titleID}.nsp"), outputPath);
            return outputPath;
        }

        static string ParseNCAOutput(string output, string type)
        {
            var line = output.Split('\n')
                .FirstOrDefault(l => l.Contains($"Created {type} NCA:"));

            return line?.Split(':').Last().Trim();
        }
        #endregion
    }
}
