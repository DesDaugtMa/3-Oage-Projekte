# Projektplanung

## Controller
Für die Beidem Komponenten *Kategorie* und *Eintrag* wird jeweils ein Controller benötigt. Diese beinhalten die nötigen CRUD-Aktionen und kommunizieren mit den Views. Bei dem *Kategorie*-Controller wird noch eine sonder-Logik für das löschen benötigt, da die Einträge einer zu löschenden Kategorie in eine andere Kategorie verschoben werden sollen.

Zudem wird ein *Statistik*-Controller benötigt, um angeforderte Statistiken darstellen zu können. Dieser Controller liest nur aus der Datenbank und führt keine anderen Operationen aus.

## Views
Für *Kategorie* und *Eintrag* gib es jeweils die Views *Index*, *Create*, *Edit* und *Delete*. Das eine Kategorie einen gesonderten Lösch-Vorang benötigt, gibt es noch die View *SwitchTransactions*.

Die *Statisik* verfügt nur über eine *Index*-Ansicht, in der sich alle Statistiken befinden.

## Datenbank
Die Datenbank besteht aus zwei Tabelle: *Transaction* und *Category*, die eine 1-zu-n(Category-zu-Transactions) Verbindung haben