# kiosk-server

Touch screen kiosk with multi-platform remote control web server, using blazor and net9.

UNDER DEVELOPMENT

TODO
- user/password authentication for setup screen

Display when no Kiosk URL is defined yet:

![touch screen](https://i.imgur.com/bTQtqSe.png)

When using the external setup URL (http://x.x.x.x:5000/setup, no password authentication YET!) you can enter one or more Kiosk URLs and then either reboot or shutdown the raspberry pi.

The Kiosk URL is only shown after a reboot.

If there is only one Kiosk URL, then the software will redirect to that URL at startup.

If there is more than one Kiosk URL defined, then the software will redirect to an internal page (http://x.x.x.x:5000/kiosk) that has a tab bar at the top and an iframe filling the rest of the screen. 

The contents of the iframe is changed to the Kiosk URL, after pressing the tab button with the name of the Kiosk URL.

If you see a blank iframe with an error like 'www.msn.com refused to connect.' :
That means that the remote web server does not allow rendering inside an iframe. (via the X-Frame-Options http response header)

You won't have this problem, if you define just one Kiosk URL.

There is also a page http://x.x.x.x:5000/blank that shows a blank page.

There is also a (GET) rest api endpoint (http://x.x.x.x:5000/api/status) that returns a JSON object, containing system status data.

There are also (POST) rest api endpoints (http://x.x.x.x:5000/api/shutdown , http://x.x.x.x:5000/api/reboot , http://x.x.x.x:5000/api/screenoff and http://x.x.x.x:5000/api/screenon) NOTE that there is no authentication!

There is also a (POST) rest api endpoint http://x.x.x.x:5000/api/navigatetourl?url=xxxxxxx that ONLY works, when the kiosk screen is being displayed.

When this url is POSTed, with ANY url as query parameter (e.g. http://x.x.x.x:5000/api/navigatetourl?url=http://x.x.x.x:5000/blank) that page will be loaded into the kiosk iframe and the tab bar at the top is hidden.

When this url is POSTed, without any url as query parameter (e.g. http://x.x.x.x:5000/api/navigatetourl?url=) then the kiosk is reloaded and the tab bar at the top reappears.

![touch screen](https://i.imgur.com/Wzp5kqm.png)

![touch screen](https://i.imgur.com/cXrHx23.png)

## Test Environment

**This software works with any kind of display. I have been testing with ultra-wide displays :**

3D-printed enclosures, for these displays, can be found : https://www.printables.com/@mhwlng_888536/collections/920676

Touch Display 1920x480 (8.8 inch, IPS panel, default orientation is portrait):

https://www.aliexpress.com/item/1005003014364673.html

![touch screen](https://i.imgur.com/QWs2S9S.jpg)

![touch screen](https://i.imgur.com/GfcSTTd.jpg)

The 3 Dials have an ESP32 processor and are made by M5Stack :

https://shop.m5stack.com/products/m5stack-dial-esp32-s3-smart-rotary-knob-w-1-28-round-touch-screen

![touch screen](https://i.imgur.com/0NSFRaz.jpg)

Touch Display 1920x515 (12.6 inch, IPS panel):

https://www.aliexpress.com/item/1005001966967133.html

[3d printed modular dashboard](https://www.printables.com/@mhwlng_888536/collections/920676)

![touch screen](https://i.imgur.com/hFIeCUD.jpg)

![touch screen](https://i.imgur.com/4YI13mJ.jpg)

![touch screen](https://i.imgur.com/erLvZY7.jpg)

Touch display 3840x1100 (14 inch, IPS panel, uses usb-c connector for power and touch screen):

https://www.aliexpress.com/item/1005003332731770.html

[3d printed modular dashboard](https://www.printables.com/@mhwlng_888536/collections/920676)

![touch screen](https://i.imgur.com/MjmCNvf.jpg)

![touch screen](https://i.imgur.com/ysxHEvS.jpg)

Raspberry Pi Compute Module 4 (I only have the 8GB RAM / 16GB EMMC version)

The Waveshare CM4-NANO-B expansion board (also available on Amazon)

https://www.waveshare.com/wiki/CM4-NANO-B

The USB-C port is connected to the 5V power. (Also used to put the OS image onto the EMMC flash.)

The USB-A port is connected to the display. (To power it and also for the touch screen)

The HDMI port is connected to the display HDMI port.

Pressing the pushbutton (GPIO21) shuts down the CM4

# Installation Instructions

I mainly used this document as a guideline: (note that for the CM4 it's not exactly the same)

https://gist.github.com/fjctp/210f4e870f913416b8d0e17fd36153c2


## Install bootloader on CM4

https://www.raspberrypi.com/documentation/computers/compute-module.html

Set boot switch to on, plug in usb-c 

Download rpiboot_setup.exe, run rpiboot.exe 

Also see waveshare CM4-NANO-B wiki page.

https://github.com/raspberrypi/usbboot/raw/master/win32/rpiboot_setup.exe

- Install raspberry pi os lite 64 bit (bookworm)
- Set up wifi
- Set up ssh
- Set up an account (The instructions and various configuration files assume pi/raspberry Adjust as required.)

After connecting via ssh :
```
sudo apt-get update

sudo apt-get upgrade

sudo raspi-config

Select system \ boot+autologin \ B2 Console autologin text console

sudo apt-get install -y --no-install-recommends xserver-xorg x11-xserver-utils xinit openbox

sudo apt-get install -y --no-install-recommends chromium-browser
```

## Edit /boot/firmware/config.txt

**Note: These HDMI resolution configurations do NOT work on Raspberry Pi 5 / CM5 !**

For Touch Display 1920x480 (portrait orientation, default) :
```
dtoverlay=vc4-fkms-v3d # note that this was vc4-kms-v3d before !!!!!

max_framebuffer_height=1920
hdmi_timings=480 1 48 32 80 1920 0 3 10 56 0 0 0 60 0 75840000 3
hdmi_group=2
hdmi_mode=87

#otg_mode=1
dtoverlay=dwc2,dr_mode=host
dtoverlay=gpio-shutdown,gpio_pin=21

gpu_mem=256
```

For Touch Display 1920x480 (landscape orientation, rotate 90&deg;) also add:
```
display_hdmi_rotate=1
```

For landscape orientation: the touchscreen also needs to be rotated 90&deg; :

Edit /usr/share/X11/xorg.conf.d/40-libinput.conf

Add the TransformationMatrix option to the existing touchscreen InputClass:
```
Section "InputClass"
        Identifier "libinput touchscreen catchall"
        MatchIsTouchscreen "on"
        Option "TransformationMatrix" "0 1 0 -1 0 1 0 0 1"
        MatchDevicePath "/dev/input/event*"
        Driver "libinput"
EndSection
```

For Touch Display 1920x515 :
```
dtoverlay=vc4-fkms-v3d # note that this was vc4-kms-v3d before !!!!!

hdmi_group=2
hdmi_mode=87
hdmi_cvt=1920 515 60 6 0 0 0

#otg_mode=1
dtoverlay=dwc2,dr_mode=host
dtoverlay=gpio-shutdown,gpio_pin=21

gpu_mem=256
```

For Touch Display 3840x1100 :
```
dtoverlay=vc4-fkms-v3d # note that this was vc4-kms-v3d before !!!!!

hdmi_enable_4kp60=1
hdmi_group=2
hdmi_mode=87
hdmi_cvt=3840 1100 60

#otg_mode=1
dtoverlay=dwc2,dr_mode=host
dtoverlay=gpio-shutdown,gpio_pin=21

gpu_mem=256
```

## Edit /etc/xdg/openbox/autostart

```
xset s off
xset s noblank
xset -dpms

setxkbmap -option terminate:ctrl_alt_bksp

sed -i 's/"exited_cleanly":false/"exited_cleanly":true/' ~/.config/chromium/'Local State'
sed -i 's/"exited_cleanly":false/"exited_cleanly":true/; s/"exit_type":"[^"]\+"/"exit_type":"Normal"/' ~/.config/chromium/Default/Preferences

# delete all chromium cached data
rm -rf ~/.cache/chromium

# delete cookies
#rm -rf ~/.config/chromium

chromium-browser --noerrdialogs --disable-infobars --kiosk 'http://127.0.0.1:5000'
```

For the 3840x1100 screen, you can increase the zoom level of chromium using --force-device-scale-factor=1.5 on the command line.

If the web page checks for the dark mode system setting, --force-dark-mode --enable-features=WebContentsForceDark can be added to the command line.

To disable the cache mechanism, --disk-cache-dir=/dev/null can be added to the command line.

Some chromium performance related flags can be found here :

https://github.com/Botspot/pi-apps/blob/master/apps/Better%20Chromium/install

some more chromium flags can be found here :

https://itnext.io/raspberry-pi-read-only-kiosk-mode-2022-complete-tutorial-df7fc051fdaf

```
chromium-browser --ignore-gpu-blacklist --enable-checker-imaging --cc-scroll-animation-duration-in-seconds=0.6 --disable-quic --enable-tcp-fast-open --enable-experimental-canvas-features --enable-scroll-prediction --enable-simple-cache-backend --max-tiles-for-interest-area=512 --num-raster-threads=4 --default-tile-height=512 --enable-features=VaapiVideoDecoder,VaapiVideoEncoder --disable-features=UseChromeOSDirectVideoDecoder,TouchpadOverscrollHistoryNavigation --enable-accelerated-video-decode --enable-low-res-tiling --process-per-site --start-fullscreen --disable-translate --no-first-run --fast --fast-start --disable-features=TranslateUI --password-store=basic --disable-pinch --overscroll-history-navigation=disabled --noerrdialogs --disable-infobars --kiosk 'http://127.0.0.1:5000'
```

## Edit ~/.profile

```
[[ -z $DISPLAY && $XDG_VTNR -eq 1 ]] && startx -- -nocursor
```

## Web Server

Copy all the web server application files and subdirectories to ~/kiosk-server

Make application runnable using :
```
sudo chmod +x ~/kiosk-server/kiosk-server
```

Install the application as a service (Adjust kiosk-server.service if user or directory is different) :
```
sudo systemctl stop kiosk-server

sudo cp ~/kiosk-server/kiosk-server.service /etc/systemd/system/kiosk-server.service

sudo systemctl daemon-reload

sudo systemctl enable kiosk-server

sudo systemctl start kiosk-server
```

Check if service is running ok :
```
sudo systemctl status kiosk-server

or

sudo journalctl -u kiosk-server 
```

## Visual Studio Publish Action

When using 'Publish' -> Visual Studio automatically synchronises all files to ~/kiosk-server using WinSCP.

Adjust paths, ip address, user and password in .csproj file as required :
```
<Target Name="PiCopy" AfterTargets="AfterPublish">
   <Exec Command="&quot;C:\Program Files (x86)\WinSCP\WinSCP.com&quot; /command &quot;open sftp://pi:raspberry@192.168.2.36/&quot; &quot;synchronize remote C:\dotnet\projects\kiosk-server\kiosk-server\bin\Release\net9.0\publish /home/pi/kiosk-server/&quot; &quot;exit&quot;" />
</Target>
```

Alternatively, you could also copy all the files using pscp, that comes with putty:

```
Target Name="PiCopy" AfterTargets="AfterPublish">
   <Exec Command="pscp -r -pw raspberry C:\dotnet\projects\kiosk-server\kiosk-server\bin\Release\net9.0\publish\ pi@192.168.2.36:/home/pi/kiosk-server/" />
</Target>
```

First stop the web server, before updating the files :
```
sudo systemctl stop kiosk-server
```

## Home Assistant Dashboards

home assistant doesn't work inside an iframe, until you add to the configuration.yaml

```
http:
  use_x_frame_options : false
```

There is no on-screen keyboard, so an auto login mechanism is required:

Add a new user 'Kiosk'

The user id can be found on the details pop-up:
![home assistant](https://i.imgur.com/MzeJlGT.png)

Then, add the IP address of the kiosk as trusted network and the Kiosk user id as a trusted user to configuration.yaml:
```
# Allow login without password from local network
homeassistant:
  auth_providers:
    - type: trusted_networks
      trusted_networks:
        - 192.168.2.36/32
      trusted_users:
        192.168.2.36:
          - dacfc03879144b31b57104cc00f6a1a2 ## specific user for kiosk
      allow_bypass_login: true
    - type: homeassistant
```

You can use the normal home assistant header navigation buttons, to switch between dashboards. 

Or you can also add each dashboard URL separately to the Kiosk URL List:

![touch screen](https://i.imgur.com/cXrHx23.png)

![home assistant](https://i.imgur.com/xlLNF75.jpg)

I installed the 'Kiosk Mode' HACS frontend repository. See https://github.com/NemesisRE/kiosk-mode

Now, you can use the kiosk query parameter, to hide the header and sidebar on the dashboard:

For example : http://192.168.2.73:8123/lovelace/home?kiosk

Note, that this only works correctly, if you hide the sidebar by default, for the Kiosk user:

![home assistant](https://i.imgur.com/pKVELn4.png)

## Transfer system status data to Home Assistant

```

sensor:
  - platform: rest
    name: kiosk_sensors
    scan_interval: 60
    resource: http://192.168.2.38:5000/api/status
    json_attributes:
        - disk
        - temperature
        - memory
        - cpu
    value_template: "OK"

  - platform: template
    sensors:
      kiosk_temperature:
        unique_id: kiosk_temperature
        friendly_name: "CPU Temperature"
        value_template: "{{ state_attr('sensor.kiosk_sensors', 'temperature')['cpuTemperature'] | round(1) }}"
        device_class: temperature
        unit_of_measurement: "°C"
      kiosk_cpu_percent:
        unique_id: kiosk_cpu_percent
        friendly_name: "CPU Usage"
        value_template: "{{ state_attr('sensor.kiosk_sensors', 'cpu')['cpuUsage'] | round(1)}}"
        unit_of_measurement: "%"
```


## Turn off kiosk when PC is turned off

```

rest_command:
  kiosk_off:
    url: "http://192.168.2.38:5000/api/shutdown"
    method: POST

binary_sensor:
  - platform: ping
    host: 192.168.2.35
    name: dev5_ping
    scan_interval: 60
  - platform: template
    sensors:
      dev5_online:
        unique_id: dev5_online
        friendly_name: "DEV5 Online"
        delay_off:
          minutes: 2
        value_template: "{{ states('binary_sensor.dev5_ping')}}"
   
automation
   
- id: '...........'
  alias: kiosk off when DEV5 off
  description: ''
  trigger:
  - platform: state
    entity_id:
    - binary_sensor.dev5_online
    from: 'on'
    to: 'off'
  condition: []
  action:
  - service: rest_command.kiosk_off
    data: {}
  mode: single
        
```
