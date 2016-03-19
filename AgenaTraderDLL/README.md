#Einbinden einer DLL in den Agena Trader
Ursprüngliches Posting im Forum: http://www.tradeescort.com/phpbb_de/viewtopic.php?f=18&t=1342

Ich greife das Thema nochmal auf, da das Einbinden von DLLs sicherlich eine extrem nützliche Funktion ist. 
So kann man sich Klassen, Extensions oder Helper einmal zentral zurechtlegen und muss den Source Code nicht jeweils in die Indikatoren oder Conditions kopieren (Redundanz = Fehleranfälligkeit!).

Ich habe vorhin eine Test-DLL in den AT mit folgendem Setup eingebunden:
+ Windows 10
+ Agena Trader Andromeda 1.0.8.453
+ Visual Studio Ultimate 2013
+ Für alle Aktivitäten verwenden wir .NET 3.5 Framework 
+ Visual Studio wurde als Administrator ausgeführt, da sonst Probleme beim Kopieren in den GAC auftreten.

##Erstellen einer DLL in Visual Studio
Unsere Test-DLL besteht aus folgendem Code:
```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgenaTraderDLL
{
    public static class  ExternalHelper   {
        public static String Test() {
            return "Hello World!";
        }
    }
}
```

In den Projekteinstellungen muss das Projekt bevor wir kompilieren zuerst signiert werden! (siehe Attachment gelber Pfeil).

Wie in Postings vorhin schon angesprochen müssen wir die von uns erstellte DLL über gacutil.exe in den GAC (Global Assembly Cache) kopieren. Eine Alternative ist das Erstellen eines MSI Packages. Die gacutil Variante ist während der Entwicklung die schnellere Variante.
Da wir .Net 3.5 einsetzen, verwenden wir gacutil aus folgendem Ordner: C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\gacutil.exe

Um sich etwas Arbeit beim kopieren in den GAC zu ersparen kann der Visual Studio Post Build Prozess verwendet werden, so kopiert Visual Studio nach jedem Build die DLL in den GAC.
(siehe Attachment oranger Pfeil).

```
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\gacutil.exe"  /i "$(TargetPath)"
```

Nun kann die Test-DLL kompiliert werden und wird automatisch in den GAC kopiert.

Um zu kontrollieren ob die DLL auch wirklich im GAC ist kann folgender Befehl verwendet werden:
```
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\gacutil.exe"  /l "AgenaTraderDLL"
```

Falls man händisch die DLL wieder aus dem GAC entfernen möchte, kann folgender Befehl verwendet werden:
```
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\gacutil.exe"  /u "AgenaTraderDLL"
```

##Verwendung unserer DLL im AT

Wir starten AT und klicken wie schon im obigen Postings beschrieben auf: Tools => Programmierung => Programmier Referenzen.
Im Select References Fenster erkennen wir unsere DLL im GAC (gelber Pfeil).
Über den Karteireiter Browse (blauer Pfeil) laden wir unsere DLL seperat in den AT und werden anschließend im unteren Fenster den Pfad zur lokalen DLL wiederfinden (grüner Pfeil).

Wenn wir das korrekte Using Statement in Indikatoren oder Conditions hinzufügen, können wir auf die Funktionen unserer DLL zugreifen. 
```
using AgenaTraderDLL;
```

Im OnBarUpdate Event greifen wir auf den statischen Helper zu und schreiben den Wert aus unserer DLL in die Ausgabe des AT.

```
protected override void OnBarUpdate() {
     String helloDLLworld = ExternalHelper.Test();
     Print(helloDLLworld);
}
```

Zu guter Schluß einmal alles im AT kompilieren und fertig.
Viel Spaß!
