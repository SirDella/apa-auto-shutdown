# APA AutoShutdown
Simple automatic shutdown app based on network and CPU usage.

Allows you to leave your PC while it's working and let it shut down/sleep on its own once finished.

# Use cases
  - Downloading large files/games
  - Rendering
  - Syncing backups
  - Recording meetings/streams

# Features
  - CPU/Network/Both measurement
  - Timer by minutes or exact hour
  - Shut down/sleep
  - Options for automation
  - Average measurement for stable readings

# How to use
Depending on what your are doing you will need to tweak the settings to an optimal value.
The default settings should work for any download/render, but if you need faster response times or your usage never goes above the default values, you will have to lower them.

By default, the measurement module and the timer module are independent, which means the one that goes off first is going to turn off the computer. This allows you to, for example, turn off the computer at a certain time even if it hasn't finished doing its work. 

With the "chain shutdowns" button, both modules have to go off in order to turn it off. So now, the computer won't turn off before the set time, and only if it has finished its work.

# Download (Windows)
https://github.com/SirDella/apa-auto-shutdown/raw/master/intento%20de%20auto%20apagado/bin/Release/APA.exe
