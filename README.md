# P2P-Monitor-V2

This is a little Windows Forms tool I put together to keep an eye on DreamBot log files and send anything interesting over to Discord. It’s mainly for tracking chat, quests, and failure events in real time.  
The app has a Material Design style interface, saves your settings, and can even grab screenshots of the bot client if you want.

## [Click Here For P2P Monitor Discord Invite](https://discord.gg/EpuaMTCzx5)

## Download

[Click Here to download the latest release!](https://github.com/MrTysonsHacks/P2P-Monitor/releases/latest)

---
## Usage

[![IMAGE ALT TEXT HERE](https://i.imgur.com/w24sqZM.png)](https://www.youtube.com/watch?v=52tY_gN0mcM)

1. Launch the app.  
2. Pick your log folders and drop in your webhooks.  
3. Set mentions (optional) and the check interval.  
4. Tick the things you want to monitor (chat, quests, errors, etc).  
5. Hit **Start** and watch the console update.  
6. Use the **Test** buttons to make sure Discord is set up right. 
 
---

## Features

## Features

### Log Watching
- Monitor one or more folders for `logfile-*.log` (multiple paths supported via `;`).
- Remembers read offsets so it never re-sends old entries.
- Heartbeat + “jump to end” on start so only new activity is processed.

### Task & Activity Detection
- Detects **NEW TASK** blocks with priority
- Safety checks for false positive tasks.

### Discord Integration
- Sends events via webhooks with optional per-category overrides:
  - **Default Webhook** (global) + **Chat**, **Quest**, **Task**, **Error**, **Selfie** channels.
- Smart fallback: if a specific channel is empty, it **falls back to Default**.
- Editable JSON embed templates for Chat/Quest/Task/Error.
- Mentions supported (`{mention}`), plus test buttons for quick verification.

### Screenshots & Selfie Mode
- Optional **auto-screenshot** of DreamBot when events occur.
- **Selfie mode**: periodic window capture to a dedicated channel.
- Optional auto-cleanup of screenshots after successful send.

### Failure Detection
- Regex-based **quest/skill/combat** failure triggers & reasons managed in google sheets for realtime updating.
- Dedicated error embeds (also fall back to Default when no error webhook is set).

### UI / UX
- **Minimize to tray** with tray menu (Show / Start / Stop / Exit).
- Dark/Light theme with persistent preference.
- Built-in log console with timestamps, clear button, and manual heartbeat.
- Handy quick links (wiki, forums, discords, etc.).

### Reliability & Performance
- FileSystemWatcher-based updates with debounced processing.
- Start-up “announce latest per folder” + double sync to ensure clean state.
- Robust guards around IO, parsing, and Discord posting.

### Configuration & Persistence
- All toggles, intervals, webhook URLs, and templates are saved to settings.
- CLI/launcher support (via the CLI Creator) to spin up accounts with your preferred flags.

### Updater
- Built-in update check with version display so you can keep current easily.

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

## License

This project is under the **GNU General Public License v2.0 (GPL-2.0)**.  
See the [LICENSE](LICENSE) file for the full text.  
