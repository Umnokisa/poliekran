using System.Globalization;
using Microsoft.Win32;

namespace Poliekran
{
    static class UiText
    {
        public static readonly bool English = DetectEnglish();

        public static string S(string ru, string en) => English ? en : ru;
        public static string Seconds(int seconds) => seconds == 0 ? S("не листать", "do not advance") : seconds + S(" с", " s");

        static bool DetectEnglish()
        {
            try
            {
                using (var k = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\Poliekran"))
                {
                    var language = k?.GetValue("Language") as string;
                    if (string.Equals(language, "en", System.StringComparison.OrdinalIgnoreCase)) return true;
                    if (string.Equals(language, "ru", System.StringComparison.OrdinalIgnoreCase)) return false;
                }
            }
            catch { }
            return !string.Equals(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, "ru", System.StringComparison.OrdinalIgnoreCase);
        }

        public static string AppName => S("Полиэкран", "Poliekran");
        public static string FullScreen => S("Полный экран", "Full screen");
        public static string Window => S("Окно", "Window");
        public static string File => S("Файл", "File");
        public static string Monitor => S("Монитор", "Monitor");
        public static string Slide => S("Слайд", "Slide");
        public static string Status => S("Статус", "Status");
        public static string Settings => S("Настройки", "Settings");
        public static string Reload => S("Перезапускать показ, когда файл изменился", "Restart the show when the file changes");
        public static string AdvanceSlides => S("Слайды без времени смены листать через:", "Advance slides without timing after:");
        public static string StartFile => S("Запустить файл…", "Start file…");
        public static string RefreshMonitors => S("Обновить мониторы", "Refresh monitors");
        public static string Previous => S("< Назад", "< Previous");
        public static string Next => S("Вперёд >", "Next >");
        public static string Restart => S("Перезапустить", "Restart");
        public static string Stop => S("Остановить", "Stop");
        public static string StopAll => S("Остановить все", "Stop all");
        public static string StopWithEsc => S("Esc в показе останавливает его", "Esc stops the show");
        public static string SelectShow => S("Выберите показ в списке.", "Select a show in the list.");
        public static string StartFailed => S("Не удалось запустить", "Could not start the show");
        public static string ShowCrashed => S("упал, повтор через ", "failed, retry in ") + "{0}" + S(" с", " s");
        public static string Restarting => S("перезапуск", "restarting");
        public static string Running => S("идёт", "running");
        public static string WatchingFile => S("следит за файлом", "watching file");
        public static string PrimaryMonitor => S("основной", "primary");
        public static string Position => S("позиция", "position");
        public static string PowerPointFiles => S("Файлы PowerPoint", "PowerPoint files");
        public static string AddInNotStarted => S("Надстройка не запустилась. Подробности в журнале:\n", "The add-in did not start. See the log for details:\n");
        public static string PreviousShowStarting => S("Предыдущий показ ещё запускается. Подождите пару секунд и повторите.", "The previous show is still starting. Wait a few seconds and try again.");
        public static string ShowRestarting(string title) => S("Показ «" + title + "» уже перезапускается. Подождите несколько секунд.", "The show \"" + title + "\" is already restarting. Wait a few seconds.");
    }
}
