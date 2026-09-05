using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.Plugins
{
    public class Aloft : SteamCMDGameServer
    {
        // Game server properties
        public Aloft(ServerConfig serverData) : base(serverData) { }

        public override void CreateServerProp()
        {
            var serverPropPath = ServerPath.GetServersServerFiles(_serverData.ServerID, "server.properties");
            
            if (File.Exists(serverPropPath))
                return;

            string configContent = @"# Aloft Game Server Configuration
server_name=My Aloft Server
server_port=7777
server_query_port=27015
max_players=32
difficulty=Normal
pvp_enabled=true
enable_backups=true
backup_interval=3600
auto_save=true
auto_save_interval=300
";

            File.WriteAllText(serverPropPath, configContent);
        }

        public override bool Install(string path)
        {
            try
            {
                var steamCmdArguments = string.Format("+login anonymous +app_update 2313410 validate +quit");

                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = SteamCMDPath,
                        Arguments = steamCmdArguments,
                        WorkingDirectory = path,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                p.Start();
                p.WaitForExit();

                return p.ExitCode == 0;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
        }

        public override bool Update(bool validate = false, string custom = "", string custom_args = "")
        {
            try
            {
                var steamCmdArguments = string.Format("+login anonymous +app_update 2313410 {0} +quit", 
                    validate ? "validate" : "");

                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = SteamCMDPath,
                        Arguments = steamCmdArguments,
                        WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                p.Start();
                p.WaitForExit();

                return p.ExitCode == 0;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
        }

        public override bool Start()
        {
            try
            {
                string serverPath = ServerPath.GetServersServerFiles(_serverData.ServerID);
                string exePath = Path.Combine(serverPath, "Aloft", "Binaries", "Win64", "AloftServer.exe");

                if (!File.Exists(exePath))
                {
                    Error = "Server executable not found";
                    return false;
                }

                string args = string.Format("-port={0} -queryport={1} -maxplayers={2}",
                    _serverData.GetGamePort(),
                    _serverData.GetQueryPort(),
                    _serverData.ServerMaxPlayer);

                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = args,
                        WorkingDirectory = serverPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                p.Start();
                _serverData.ProcID = p.Id;

                return true;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
        }

        public override bool Stop()
        {
            try
            {
                Process.GetProcessById(_serverData.ProcID).Kill();
                return true;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
        }

        public override bool IsInstalled()
        {
            string exePath = Path.Combine(
                ServerPath.GetServersServerFiles(_serverData.ServerID),
                "Aloft", "Binaries", "Win64", "AloftServer.exe");

            return File.Exists(exePath);
        }

        public override bool IsRunning()
        {
            try
            {
                return Process.GetProcessById(_serverData.ProcID) != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
