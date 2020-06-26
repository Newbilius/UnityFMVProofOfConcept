# Что это такое

Попытка сделать на Unity 2019.4 (LTS) основу для простейшего интерактивного фильма - выбираем один из вариантов ответа, смотрим видеоролик, выбираем следующий вариант ответа, смотрим следующее видео и так далее. Никаких излишеств вроде QTE или чего-то ещё. Готовый проект можно скачать [в разделе релизов](https://github.com/Newbilius/UnityFMVProofOfConcept/releases). Игр должна нормально управляться и геймпадом (xbox или современных версий Playstation), и клавиатурой, мышкой.

С удовольствием выслушаю советы по улучшению кода и работу с Unity.

# Как готовить сценарий

Сценарий готовится в редакторе [Twine](https://github.com/klembot/twinejs/releases/) с последующим экспортированием в JSON [вот этим аддоном](https://github.com/DigitalCarleton/Prairie/wiki/Exporting-Data-from-Twine-to-JSON). 

![внешний вид сценария](/screens_for_github/screens.png)

В названии блока можно в скобках указать айдишник - он потом будет доступен из кода.

![иллюстрация](/screens_for_github/scene_id.png)

В первой строки каждой сцены указывается имя видеофайла.

![иллюстрация](/screens_for_github/file_name.png)

В последующих строках можно указывать переходы на другие сцены в формате [Название сцены] или [Отображаемое название сцены->Реальное название сцены] или управлять фоновой музыкой.

Музыка задаётся с помощью символа "$":
* $композиция, включаемая в начале сцены
* $$композиция, включаемая в конце сцены

Вместо названия композиции можно указать "NULL" - тогда музыка будет остановлена.

Можно не указывать ни одной строки с управлением музыкой - тогда продолжит зациклено играть предыдущая композиция.

Если нужно как-то влиять на логику сценария из кода - это можно делать в [GameScriptsProvider.cs](Assets/Scripts/Gameplay/GameScriptsProvider.cs). Это осознанное решение: в принципе Twine поддерживает элементы скриптования (условия, переменные), но я хотел всё-таки логику игры задавать на С#, а не на отдельном собственном языке.

Недостатки подхода очевидны: сценарий делается сбоку от собственно движка, нет возможности провалидировать наличие выбранного видео и аудио, нельзя нормально задать кастомные поля и т.п.

# Todo (доделать в ближайшее время, например никогда)

* Добавить анимацию выбранной кнопки (в соседних ветках есть эксперименты на эту тему)
* Система сохранения и загрузки на случай длинного фильма
* Добавить в настройки опцию "отключение мышки"?
* Скрывать курсор при управлении с клавиатуры/геймпада (и возвращать при движениях мышки)
* Добавить поддержку локализации текстов, субтитров и звуковой дорожки
* При смене музыки делать кроссфэйд, при отключении музыку - уменьшать громкость плавно

# Права и обязанности

Хотите использовать этот код для создания собственной игры - пожалуйста, никаких ограничений и обязательств. Но имейте ввиду, для тестовых видео использованы материалы из Doom - у меня нет прав на них, они тут использованы в рамках добросовестного использования, и в случае обращения правообладателей эти данные будут удалены из репозитория.
