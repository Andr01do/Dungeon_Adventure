using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;
    public string currentLanguage = "en";
    public event Action OnLanguageChanged;

    private Dictionary<string, Dictionary<string, string>> localizedText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLocalizedText();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        string savedLang = PlayerPrefs.GetString("SelectedLanguage", "en");
        SetLanguage(savedLang);
    }

    public void SetLanguage(string langCode)
    {
        currentLanguage = langCode;
        PlayerPrefs.SetString("SelectedLanguage", langCode);
        OnLanguageChanged?.Invoke();
    }

    public string GetTranslation(string key)
    {
        if (localizedText != null &&
            localizedText.ContainsKey(currentLanguage) &&
            localizedText[currentLanguage].ContainsKey(key))
        {
            return localizedText[currentLanguage][key];
        }
        return key;
    }

    private void LoadLocalizedText()
    {
        localizedText = new Dictionary<string, Dictionary<string, string>>();

        var en = new Dictionary<string, string>();
        var ua = new Dictionary<string, string>();

        // --- ENGLISH DATA ---
        en.Add("tittle_ha", "Hryshchuk Andriy");
        en.Add("tittle_sv", "Svintsytskyi Vitaliy");
        en.Add("tittle_vs", "Sukhomlyn Veronika");
        en.Add("tittle_testers", "Testers:");

        en.Add("msg_boss_drop", "Boss Drop");
        en.Add("msg_win_final", "VICTORY!\nTotal Time: {0}\n\n");
        en.Add("msg_floor_complete", "Floor {0} Cleared!\nTime: {1}");
        en.Add("hud_floor", "FLOOR {0} / {1}");

        en.Add("msg_locked", "Locked");
        en.Add("art_book", "Book");
        en.Add("art_book_desc", "10% exp");
        en.Add("art_boots", "Boots");
        en.Add("art_boots_desc", "25% stamina");
        en.Add("art_charm", "Charm");
        en.Add("art_charm_desc", "10% hp and 10% stamina");
        en.Add("art_crown", "Crown");
        en.Add("art_crown_desc", "-50% hp and +25% attack");
        en.Add("art_feather", "Feather");
        en.Add("art_feather_desc", "15% speed");
        en.Add("art_gloves", "Gloves");
        en.Add("art_gloves_desc", "10% attack");
        en.Add("art_lolipop", "Lolipop");
        en.Add("art_lolipop_desc", "10% hp");
        en.Add("art_salo", "Salo");
        en.Add("art_salo_desc", "25% hp");
        en.Add("art_sandclock", "Sandclock");
        en.Add("art_sandclock_desc", "10% attack speed");
        en.Add("art_turtle", "Turtle");
        en.Add("art_turtle_desc", "10% stamin regen");


        en.Add("ach_archer_mas", "Archer Master");
        en.Add("ach_archer_desc", "Beat the game in hard difficulty with Archer");
        en.Add("ach_beat_easy", "Beat Easy");
        en.Add("ach_beat_easy_desc", "Beat the game in easy difficulty");
        en.Add("ach_beat_medium", "Beat Medium");
        en.Add("ach_beat_medium_desc", "Beat the game in medium difficulty");
        en.Add("ach_beat_hard", "Beat Hard");
        en.Add("ach_beat_hard_desc", "Beat the game in hard difficulty");
        en.Add("ach_mage_mas", "Mage Master");
        en.Add("ach_mage_mas_desc", "Beat the game in hard difficulty with Mage");
        en.Add("ach_knight_mas", "Knight Master");
        en.Add("ach_knight_mas_desc", "Beat the game in hard difficulty with Knight");
        en.Add("ach_dung_slayer", "Dungeon Slayer");
        en.Add("ach_dung_slayer_desc", "Kill 100 enemies");
        en.Add("ach_first_blood", "First Blood");
        en.Add("ach_first_blood_desc", "Make a first blood");
        en.Add("ach_rank_s", "Rank S");
        en.Add("ach_rank_s_desc", "Clear the level 100%");

 
        en.Add("title_upgrade", "What to upgrade?");
        
        en.Add("title_victory", "Victory!");
        en.Add("lbl_destroyed", "Destroyed");

        en.Add("lbl_hp", "HP");               
        en.Add("lbl_exp", "EXP");             
        en.Add("lbl_stmn", "STMN");

        en.Add("lbl_music_menu", "Music in menu");
        en.Add("lbl_language", "Language:");

        en.Add("lbl_total_bosses", "Total Bosses");
        en.Add("lbl_total_enemies", "Total Enemies");
        en.Add("lbl_total_runs", "Total Runs");

        en.Add("title_char_select", "Character Select"); 
        en.Add("title_difficulty", "Difficulty");        
        en.Add("lbl_artifacts", "Artifacts");

        en.Add("btn_continue", "Continue");
        en.Add("btn_start", "Start Game");
        en.Add("btn_achievements", "Achievements");
        en.Add("btn_settings", "Settings");
        en.Add("btn_exit", "Exit");
        en.Add("btn_credits", "Credits");
        en.Add("btn_to_menu", "To Menu");
        en.Add("btn_main_menu", "Main Menu");
        en.Add("btn_choose", "Choose");
        en.Add("btn_delete_data", "Delete Data");
        en.Add("btn_yes", "Yes");
        en.Add("btn_no", "No");
        en.Add("btn_restart", "Restart");
        en.Add("btn_next", "Next");
        en.Add("btn_resume", "Resume");
        en.Add("btn_reload", "Reload");

        en.Add("lbl_mage", "Mage");
        en.Add("lbl_archer", "Archer");
        en.Add("lbl_knight", "Knight");

        en.Add("lbl_easy", "Easy");
        en.Add("lbl_medium", "Medium");
        en.Add("lbl_hard", "Hard");

        en.Add("lbl_attack", "Attack");
        en.Add("lbl_stamina", "Stamina");
        en.Add("lbl_health", "Health");
        en.Add("lbl_floor", "Floor");
        en.Add("msg_no_save", "NO DATA");

        localizedText.Add("en", en);


        // --- UKRAINIAN DATA ---
        ua.Add("tittle_ha", "Грищук Андрій");
        ua.Add("tittle_sv", "Свінцицький Віталій");
        ua.Add("tittle_vs", "Сухомлин Вероніка");
        ua.Add("tittle_testers", "Тестери:");

        ua.Add("msg_boss_drop", "Нагорода з боса");
        ua.Add("msg_win_final", "ПЕРЕМОГА!\nЗагальний час: {0}\n\n");
        ua.Add("msg_floor_complete", "Поверх {0} зачищено!\nЧас: {1}");
        ua.Add("hud_floor", "ПОВЕРХ {0} / {1}");

        ua.Add("msg_locked", "Заблоковано");
        ua.Add("art_book", "Книжка");
        ua.Add("art_book_desc", "10% досвіду");
        ua.Add("art_boots", "Чоботи");
        ua.Add("art_boots_desc", "25% витривалості");
        ua.Add("art_charm", "Талісман");
        ua.Add("art_charm_desc", "10% оз і 10% витривалості");
        ua.Add("art_crown", "Корона");
        ua.Add("art_crown_desc", "-50% оз і +25% атаки");
        ua.Add("art_feather", "Перо");
        ua.Add("art_feather_desc", "15% швидкості");
        ua.Add("art_gloves", "Рукавички");
        ua.Add("art_gloves_desc", "10% атаки");
        ua.Add("art_lolipop", "Чупачупс");
        ua.Add("art_lolipop_desc", "10% оз");
        ua.Add("art_salo", "шматок сала");
        ua.Add("art_salo_desc", "25% оз");
        ua.Add("art_sandclock", "Пісочний годинник");
        ua.Add("art_sandclock_desc", "10% швидкості атаки");
        ua.Add("art_turtle", "Черепашка");
        ua.Add("art_turtle_desc", "10% відновлення витривалості");

        ua.Add("ach_archer_mas", "Лучник Мастер");
        ua.Add("ach_beat_easy", "Легка складність");
        ua.Add("ach_beat_medium", "Середня складність");
        ua.Add("ach_beat_hard", "Важка складність");
        ua.Add("ach_mage_mas", "Маг Мастер");
        ua.Add("ach_knight_mas", "Лицар Мастер");
        ua.Add("ach_dung_slayer", "Вбивця Підземель");
        ua.Add("ach_first_blood", "Перша кров");
        ua.Add("ach_rank_s", "Ранг S");
        ua.Add("ach_archer_desc", "Пройти гру на важкій складності за Лучника");
        ua.Add("ach_beat_easy_desc", "Пройти гру на легкій складності");
        ua.Add("ach_beat_medium_desc", "Пройти гру на середній складності");
        ua.Add("ach_beat_hard_desc", "Пройти гру на важкій складності");
        ua.Add("ach_mage_mas_desc", "Пройти гру на важкій складності за Мага");
        ua.Add("ach_knight_mas_desc", "Пройти гру на важкій складності за Лицаря");
        ua.Add("ach_dung_slayer_desc", "Вбити 100 ворогів");
        ua.Add("ach_first_blood_desc", "Пролити першу кров");
        ua.Add("ach_rank_s_desc", "Зачистити рівень на 100%");

        ua.Add("title_upgrade", "Що покращити?");

        ua.Add("title_victory", "Перемога!");
        ua.Add("lbl_destroyed", "Знищено");

        ua.Add("lbl_hp", "ОЗ");                 
        ua.Add("lbl_exp", "Досвід");             
        ua.Add("lbl_stmn", "Стаміна");

        ua.Add("lbl_music_menu", "Музика в меню");
        ua.Add("lbl_language", "Мова:");

        ua.Add("lbl_total_bosses", "Всього Босів");
        ua.Add("lbl_total_enemies", "Всього Ворогів");
        ua.Add("lbl_total_runs", "Всього Забігів");

        ua.Add("title_char_select", "Вибір Персонажа"); 
        ua.Add("title_difficulty", "Складність");       
        ua.Add("lbl_artifacts", "Артефакти");

        ua.Add("btn_continue", "Продовжити");
        ua.Add("btn_start", "Почати Гру");
        ua.Add("btn_achievements", "Досягнення");
        ua.Add("btn_settings", "Налаштування");
        ua.Add("btn_exit", "Вихід");
        ua.Add("btn_credits", "Автори");
        ua.Add("btn_to_menu", "В Меню");
        ua.Add("btn_main_menu", "Головне Меню");
        ua.Add("btn_choose", "Обрати");
        ua.Add("btn_delete_data", "Видалити Дані");
        ua.Add("btn_yes", "Так");
        ua.Add("btn_no", "Ні");
        ua.Add("btn_restart", "Заново");
        ua.Add("btn_next", "Далі");
        ua.Add("btn_resume", "Продовжити");
        ua.Add("btn_reload", "Перезавантажити");

        ua.Add("lbl_mage", "Маг");
        ua.Add("lbl_archer", "Лучник");
        ua.Add("lbl_knight", "Лицар");

        ua.Add("lbl_easy", "Легка");
        ua.Add("lbl_medium", "Середня");
        ua.Add("lbl_hard", "Важка");

        ua.Add("lbl_attack", "Атака");
        ua.Add("lbl_stamina", "Витривалість");
        ua.Add("lbl_health", "Здоров'я");
        ua.Add("lbl_floor", "Поверх");
        ua.Add("msg_no_save", "НЕМАЄ ДАНИХ");

        localizedText.Add("ua", ua);
    }
}