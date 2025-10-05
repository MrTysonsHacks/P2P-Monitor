# P2P-Monitor-V2

This is a little Windows Forms tool I put together to keep an eye on DreamBot log files and send anything interesting over to Discord. It’s mainly for tracking chat, quests, and failure events in real time.  
The app has a Material Design style interface, saves your settings, and can even grab screenshots of the bot client if you want.

---
## Download

[Click Here to download the latest release!](https://github.com/MrTysonsHacks/P2P-Monitor/releases)


## Features

### Log Watching
- Watches one or more folders for `logfile-*.log` files (you can point it at multiple dirs separated with `;`).
- Picks up:
  - Chat activity  
  - Quest completions  
  - Failures (quest, skill, combat)  
- Keeps track of where it left off so it won’t spam old entries.  

### Discord Integration
- Sends events to Discord using webhooks (separate channels for chat, quests, errors if you want).  
- Uses embed templates that you can edit yourself.  
- Supports mentions so you get notified.  
- Includes buttons to test your webhooks and make sure they work.  

### Screenshots
- Can auto-screenshot the DreamBot client when a chat event happens.  
- Optionally cleans up screenshots after they’re sent.  

### Failure Detection
- Loads regex-based triggers and reasons from a Google Sheet (so you don’t have to recompile every time you want to add something).  
- Debug mode can send the whole log file to a debug webhook if something fails.  

### Custom Embeds
- JSON templates for chat/quest events and error events.  
- Reset buttons to restore defaults if you mess them up.  
- Placeholders like `{mention}`, `{trigger}`, `{reason}`, etc.  

### UI / Misc
- Dark or light mode (your choice, it remembers your setting).  
- Built-in log console with timestamps.  
- Handy buttons for quick links (wiki, forums, discords, etc).  
- Clear log + force heartbeat check buttons.  

---

## Building

You’ll need:
- .NET Framework 4.7.2+  
- Visual Studio (2019 or newer recommended)  
- NuGet packages:
  - [MaterialSkin](https://github.com/IgnaceMaes/MaterialSkin)  
  - [Newtonsoft.Json](https://www.newtonsoft.com/json)  

Steps:
1. Clone the repo:
   ```bash
   git clone https://github.com/MrTysonsHacks/P2P-Monitor.git
   cd P2P-Monitor
   ```
2. Open the solution in Visual Studio.  
3. Restore the NuGet packages.  
4. Build (Debug or Release).  
5. Run the `.exe` in `bin/Release` or `bin/Debug`.  

---

## Config Examples

### Chat/Quest Embed
```json
{
  "content": "<@{mention}> – Detected {type}.",
  "embeds": [{
    "title": "{type}",
    "description": "Captured segment from {filename} on account {folder}",
    "color": 7506394,
    "fields": [
      {
        "name": "Segment {index}",
        "value": "{segment}",
        "inline": false
      }
    ]
  }]
}
```

### Error Embed
```json
{
  "content": "<@{mention}> – {type} Failure Detected.",
  "embeds": [{
    "title": "{type} Failure Detected",
    "description": "Captured from {filename} on account {folder}",
    "color": 16711680,
    "fields": [
      {
        "name": "Trigger",
        "value": "```{trigger}```",
        "inline": false
      },
      {
        "name": "Reason",
        "value": "{reason}",
        "inline": false
      }
    ]
  }]
}
```

### Example Log → Discord
**Log line:**
```
2025-09-08 14:22:01 Congratulations, you've completed a quest: Tutorial Island
```

**What shows up in Discord:**  
- Embed titled “Quest Completion”  
- Includes the quest name and the log filename/folder  
- Timestamp is added automatically  

---

## Screenshots

Here’s roughly what it looks like when running:

![Discord Settings](https://i.imgur.com/Pys43YR.png) 

![Monitor Settings](https://i.imgur.com/SXxEzay.png)  

![Information Tab](https://i.imgur.com/58pWLRP.png)  

![CLI Creator](https://i.imgur.com/n3fRtOu.png)

![Hamburger Menu](https://i.imgur.com/h3EUP8n.png)

![Example Embed](https://i.imgur.com/V31OXqe.png)

---

## Usage

1. Launch the app.  
2. Pick your log folders and drop in your webhooks.  
3. Set mentions (optional) and the check interval.  
4. Tick the things you want to monitor (chat, quests, errors).  
5. Hit **Start** and watch the console update.  
6. Use the **Test** buttons to make sure Discord is set up right.  

---

## License

This project is under the **GNU General Public License v2.0 (GPL-2.0)**.  
See the [LICENSE](LICENSE) file for the full text.  
