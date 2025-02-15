using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

class GTALauncher
{
    private static string configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LuckyVMenyooLauncher", "config.txt");

    [STAThread]
    static void Main()
    {
        string gtaPath = LoadOrAskForGTAPath();
        if (string.IsNullOrEmpty(gtaPath) || !Directory.Exists(gtaPath))
        {
            MessageBox.Show("Ungültiger Pfad. Beende das Programm.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        RenameFiles(gtaPath);
        StartGTA();
    }

    private static string LoadOrAskForGTAPath()
    {
        if (File.Exists(configFilePath))
        {
            try
            {
                return File.ReadAllText(configFilePath).Trim();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Lesen der Konfigurationsdatei: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        else
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Wähle deinen GTA 5 Installationsordner aus";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    if (Directory.Exists(selectedPath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
                        File.WriteAllText(configFilePath, selectedPath);
                        return selectedPath;
                    }
                    else
                    {
                        MessageBox.Show("Ungültiger Pfad ausgewählt. Bitte versuche es erneut.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        return null;
    }

    private static void RenameFiles(string directory)
    {
        var files = Directory.GetFiles(directory, "*.renamed");
        Parallel.ForEach(files, file =>
        {
            string newFileName = Path.Combine(directory, Path.GetFileNameWithoutExtension(file));
            File.Move(file, newFileName);
            Console.WriteLine($"Umbenannt: {file} -> {newFileName}");
        });
    }

    private static void StartGTA()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "steam://rungameid/271590",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Starten von GTA 5: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
