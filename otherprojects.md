#  LimitlessLED v5.0 OpenSource API
SmartThings Integration 2016: [https://github.com/peeepers/MiThings](https://github.com/peeepers/MiThings)

Siri HomeKit 2016: [https://github.com/nfarina/homebridge](https://github.com/nfarina/homebridge)

Home Assistant.io 2016: [https://home-assistant.io/components/light.limitlessled/](https://home-assistant.io/components/light.limitlessled/)

March 2016: Milight/LimitlessLED can now be controlled with Amazon Echo via Home Remote for Mac  [https://youtu.be/cSPBfL62XPU](https://youtu.be/cSPBfL62XPU)

September 2015: LimitlessLED Bridge v5.0 released. new [http://www.limitlessled.com/download/LimitlessLEDv5.zip">windows admin tool available here). Set Static IP address...  *NEW* Set Cloud server... *NEW* View Web Admin in Browser

September 2015: nodeJS homekit server for windows7/8/8.1/10. now with Siri voice support for limitlessled. https://github.com/nfarina/homebridge download myTouchHome from appstore, or elgato Eve.

September 2015: Now control LimitlessLED using Amazon Echo (entire room control from voice)   https://www.youtube.com/watch?v=X-SfZXhr-Uo

[Limitlessled_and_amazon_echo_skills_sdk](https://www.reddit.com/r/amazonecho/comments/3fv1yf/limitlessled_and_amazon_echo_skills_sdk/)

August 2015: Raspberry PI 2: NodeJS opensource github code: https://github.com/oeuillot/node-milight

August 2015: Raspberry PI 2: NodeJS opensource github code: https://github.com/mwittig/node-milight-promise

August 2015: Raspberry PI 2: npm install homebridge   https://github.com/nfarina/homebridge

July 2015: Raspberry PI code for Dual White led control... scroll to the end of this page to see more info.
http://servernetworktech.com/2014/09/limitlessled-wifi-bridge-4-0-conversion-raspberry-pi/

July 2015: SPI direct bulb communication protocol documentation added. There are 3 stages to the wifi bridge   wifi->via UART->microchip mcu->via SPI->2.4Ghz PL1176 RF transceiver.

You can use a PL1176 (or compatible LT9800) 2.4Ghz transceiver and control individual bulbs directly after you have synced your bulb to the remoteID. see the commandset at the end of this page.

April 2015: Atef has created a cool plugin for the popular eventghost program, here is the link for the plugin http://www.eventghost.net/forum/viewtopic.php?f=9&#038;t=6893

April 2015: Set a Fixed IP address using our latest LimitlessLED Wifi Bridge v4.0 Tool!!

[http://www.limitlessled.com/download/LimitlessLEDv4.zip"](http://www.limitlessled.com/download/LimitlessLEDv4.zip)

February 2015: Gary Riches has made an awesome Pebble smart watch app for LimitlessLED called Home Remote [https://apps.getpebble.com/applications/5457576cac5df3c10f000036](https://apps.getpebble.com/applications/5457576cac5df3c10f000036)

Home Remote is also available on the app store [https://itunes.apple.com/us/app/home-remote-home-automation/id926193671?mt=8](https://itunes.apple.com/us/app/home-remote-home-automation/id926193671?mt=8), and it places the commands on the notification screen for fast easy access to the lights. Siri and Apple watch support also included. It works with iphone, it supports both RGBW and Dual White bulbs and led strips.
February 2015: LimitlessLED has released another official app v1.7 with faster load times (less than 1 second), displays wifi bridge IP address, supports wifi bridge v4 with up to 1000 wifi bridges, and faster 50millisecond wifi bridge UDP discovery. [https://itunes.apple.com/us/app/limitlessled/id594759938?mt=8](https://itunes.apple.com/us/app/limitlessled/id594759938?mt=8)
you can still access the old app here if you have the old wifi bridge v1 or v2.  [https://itunes.apple.com/us/app/wifi-controller-2/id589762495?mt=8](https://itunes.apple.com/us/app/wifi-controller-2/id589762495?mt=8)

February 2015: Trevor Hart from New Zealand has just updated his Free Android app and it now allows you to control LimitlessLed bulbs lights at sunrise/sunset!

URL remains the same; [https://play.google.com/store/apps/details?id=com.tchart.scheduled](https://play.google.com/store/apps/details?id=com.tchart.scheduled)

February 2015: Epocapp has written an app in C#/C++ to control your lights. It also has an 'Ambilight' type mode as well as support for Dell's 'AlienfFX' protocol for those games that support it. He made it for fun during the Christmas vacation and thought he would get it out there, to see if people like it or not. [http://epocapp.bitbucket.org/milight/](http://epocapp.bitbucket.org/milight/)

February 2015: [http://danvy.tv/" target="_blank">Alex Danvy) from France just made a cool Library and Universal Windows App [https://github.com/danvy/miled/tree/master/src" target="_blank](https://github.com/danvy/miled/tree/master/src) Control your LimitlessLED lights with C# Windows8 Windows7 Windows8.1  Universal Windows App  Windows Phone

January 2015: Dominik Schulz and Simon Carl just made an awesome Android App for LimitlessLED, supports multiple wifi bridges and alarms. [https://play.google.com/store/apps/details?id=de.leethaxxs.rgbcontroller" target="_blank](https://play.google.com/store/apps/details?id=de.leethaxxs.rgbcontroller)

January 2015: Andrea Ghensi just finished writing a cool node-RED flow to control your lights (RGBW only) via MQTT messages, so they can be integrated in an Internet of Things environment. you can find the flow here: [http://flows.nodered.org/flow/b2cb3bdc5a81ac881d4b](http://flows.nodered.org/flow/b2cb3bdc5a81ac881d4b)

January 2015: Olli implemented a better Python library for controlling RGBW lights.

Available at [https://github.com/ojarva/python-ledcontroller](https://github.com/ojarva/python-ledcontroller) and [https://pypi.python.org/pypi/ledcontroller](https://pypi.python.org/pypi/ledcontroller)

January 2015: Muscat's Android app [https://play.google.com/store/apps/details?id=org.muscat.android.alight">download aLight org.muscat.android.alight here)

December 2014: *NEW* Android app released... [http://www.limitlessled.com/download/Milight2.0.apk">apk download here).. or [https://play.google.com/store/apps/details?id=com.lierda.wifi">googleplay download here).  [https://play.google.com/store/apps/details?id=com.cdy.client.remoteLed">old app download here).

November 2014:  how to send Nightmode to RGBW lights. send a second Off command 100 ms later. the second off command is a long press.. and it is command |= 0x80.  for example 46 XOR 80 = C6 > So send 460055 Wait 100ms Then send C60055. that is hex commands 46 00 55 and C6 00 55.

October 2014: Android Beta App: [https://play.google.com/store/apps/details?id=org.muscat.android.alight">aLight Lighting Control BETA)

October 2014: Eliot Stocker new [https://play.google.com/store/apps/details?id=tv.piratemedia.lightcontroler" target="_blank">Android LightController App) and [https://github.com/eliotstocker/Light-Controller" target="_blank">github opensource code) - Android Widgets and Music modes.

October 2014: LimitlessLED Complete PHP API [https://github.com/yasharrashedi/LimitlessLED" target="_blank">github opensource)

October 2014: Gary Riches new iPhone app 1.1 with RGB+W support is out now: [https://appsto.re/gb/hGjn3.i" target="_blank](https://appsto.re/gb/hGjn3.i) Video: [http://youtu.be/ILkjyjzKiYU" target="_blank](http://youtu.be/ILkjyjzKiYU)

October 2014: New LimitlessLED wifi bridge v4.0 [#limitleslleddiscovery">device discovery sourecode).. vb.net and C++/C#

October 2014: Updated link to Apple iPhone code for UDP sockets  [https://github.com/robbiehanson/CocoaAsyncSocket/tree/master/Source/GCD">GCDAsyncUdpSocket.m)

September 2014: Free Android App Dual White Lightbulb Scheduler/Timer [https://play.google.com/store/apps/details?id=com.tchart.scheduled](https://play.google.com/store/apps/details?id=com.tchart.scheduled)

The app allows you to schedule your lights based on times. It also allows you to control your lights via text message (I use this in conjunction with IFTTT ie turn light on when I arrive home).

September 2014: linux commandline app. [http://iqjar.com/download/jar/milight/milight_binaries.zip">milight_commandline_binaries.zip) [http://iqjar.com/download/jar/milight/milight_sources.zip">milight_commandline_sourcecode.zip)

September 2014: more python sourecode on github

September 2014: NinjaBlocks plugin, just change the port to 8899 in the driver and it works. [https://github.com/theojulienne/ninja-limitlessLED](https://github.com/theojulienne/ninja-limitlessLED)

September 2014: Updated UDP broadcast discovery for Wifi Bridges on the LAN.

September 2014: Updated how to access AT commandset on port 48899 to command wifi bridge to scan for wifi routers.

August 2014:  IFTTT [https://ifttt.com/recipes/193847-turn-off-lights-iwy-light-milight-limitless-led-when-nobody-s-home" title="IFTTT LimitlessLED" target="_blank">plugin here)

August 2014: [https://github.com/joaquincasares/python-wifi-leds" target="_blank](https://github.com/joaquincasares/python-wifi-leds)

July 2014: Trevor (from New Zealand) has written a fantastic Android  library and sample app for using the Basic4Android IDE. It is just for Dual White lights at this stage. [http://www.basic4ppc.com/android/forum/threads/limitless-led-library.42651/](http://www.basic4ppc.com/android/forum/threads/limitless-led-library.42651/)
IFTTN - If This Then Node

IFTTN is a NodeJS based server which allows you to receive actions from IFTTT. It can be used to run on a Raspberry PI in your local network to use IFTTT for further home automation and other tasks. http://sebauer.github.io/if-this-then-node/
https://ifttt.com/recipes/193847-turn-off-lights-iwy-light-milight-limitless-led-when-nobody-s-home
New Light Scheduler App for Windows 7/8/8.1/10  (uses Microsoft.NET framework)

[https://www.limitlessled.com/download/LimitlessLED_Windows_App_v2.1.11.zip">LimitlessLED_Windows_App_v2.1.11.zip)
March 2014: Added comprehensive linux Bash script commands by Vince C.

February 2014: Added Cross platform [https://github.com/shannah/CN1Sockets" target="_blank">CN1Sockets opensource library). works with Android and iOS and Java and J2ME and Blackberry and Windows Phone.

February 2014: Added Example VB.NET Full code for controlling RGBW lights.

February 2014: Added LimitlessLED Windows App for Setting wifi bridge Fixed IP, and editing the gateway address.

February 2014: Added Windows C#.NET App for control of LimitlessLED RGBW lights.

February 2014: Added Windows Phone 8 download app from the Microsoft App Store.

February 2014: Added Linux Commandline utility and opensource code for RGBW control of on, off, and brightness.

February 2014: Microsoft VB.NET Sample Code for LimitlessLED Dual White LED Music Beat Visualizer. email us for a copy.

January 2014:  Added Windows Phone example sourcecode for turning lights on and off.

January 2014: New LimitlessLED Wifi Bridge Receiver v4.0, if you received your order after 1st Jan 2014 you will have this version.
v4.0 Now contains a more advanced wifi chip that supports Wireless-N (802.11 b/g/n) and wireless b or g.

* LimitlessLED Wifi Bridge Reciever 4.0 is backwards compatible with v3.0

* Same port 8899,  you can UDP broadcast to 10.1.1.255 or 255.255.255.255 if you want all wifi bridges to receive the command on the LAN.

* Now supports Wireless-N

* Now supports wireless router outages, and auto reconnects.

* We have disabled the web admin on the new wifi bridge receiver. The Web Admin is no longer available in v4.0 of bridges using http://10.10.100.254/home.html

* If you want to configure a static ip address you can set this via mac address on your wireless router.
Jan 2014: Added <b>Arduino</b> example code: Control LimitlessLED globes with an <b>Arduino</b>.

Jan 2014: Added Color and Brightness Chart

Jan 2014: Added Python Example code for RGBW light control

Oct 2013: Added WIFI module AT commandset details for LimitlessLED Wifi Bridge version 3.0

Oct 2013: Web UI interface login details updated

Sep 2013: Released NEW LimitlessLED Wifi Bridge version 3.0

The control commands are backwards compatible with the WiFi Bridge v2.0 below.
Terms of Use: All content is copyright 2013 Limitless Designs LLC, Under no circimstances is this information to be reproduced, copied, nor distributed without written permission.

Warning: reproduction of this information without written confirmation is an offense, and is strictly enforced.

If this information is used in your programming files then you must reference the limitlessLED dev webpage using the following Text:
// Source: http://www.limitlessled.com/dev

// Copyright (2013) Limitless Designs

// If the commandset codes are utilized in any way, the LimitlessLED website url must remain in the code comments.
DETAILED DEVELOPER TECHNICAL SPECIFICATIONS

Connect iPhone/iPad direct to WiFi Bridge Receiver: Yes (WiFi adhoc AP mode)

Connect WiFi Bridge Receiver to LAN: Yes (WiFi infrastructure STA mode)
Wifi Bridge Router Web Config: http://10.10.100.254/home.html   Username: admin  Password: admin

IP Address: 10.10.100.254 (editable)

Port: 8899 (editable)

TCP/IP Mode: UDP (udp is what online games use, it is very fast and has the lowest latency)



If the app doesn't allow you to scan and find the SSID for your home network, you can use the Web User Interface to configure Wifi Bridge 3.0 STA settings on http://10.10.100.255/home.html with the username:admin  password:admin
The IP Address is now assigned automatically using DHCP. You can find the current IP address of your wifi bridge in your routers DHCP table with the same MAC address as displayed on the LimitlessLED App 3.0 first network scan screen. Or you can broadcast UDP command packets to all wifi bridges on your local network, by changing the last digit in the IP address to 255,  i.e. 10.1.1.255 

#### NEW Port Number: 8899

The Port Number most importantly has changed from 50000 to 8899
#### EXCEL VBA Fixed Code to work with and support the new Port number:

change from "Long" to "Integer"

Public Declare Function htons Lib "ws2_32" (ByVal hostshort As Integer) As Integer

## LimitlessLED OpenSource Projects

CasualLight Voice Control of LimitlessLED Lights.  by Joris studying at Waikato University, New Zealand.

[https://chrome.google.com/webstore/detail/casuallight/kehomlifkcmfagnefddfmiiemeaiogfj" title="Voice control LimitlessLED lights" target="_blank](https://chrome.google.com/webstore/detail/casuallight/kehomlifkcmfagnefddfmiiemeaiogfj)
Windows7/8 OpenSource LimitlessLED Application (commandline app written in Microsoft C#.Net).

[https://github.com/qyiet/LimitlessLED](https://github.com/qyiet/LimitlessLED) by Glen Balks from New Zealand.
HTML/Javascript LimitlessLED Application (ninjablocks).

https://github.com/jzGreen/ninja-limitlessLED <- no longer active (Dec 2015)

[http://www.youtube.com/watch?v=n8gRqzbA9bs&amp;feature=player_embedded">Youtube: NinjaBlocks/LimitlessLED and RFID)

[http://www.youtube.com/watch?feature=player_embedded&amp;v=AMaJZQns9Vc">Youtube: Limitlessled and ninja blocks)
cloverleaf LimitlessLED Ruby github opensource by brandon

[https://github.com/brandon-dacrib/cloverleaf/blob/master/app/modules/limitlessled-rgb.rb](https://github.com/brandon-dacrib/cloverleaf/blob/master/app/modules/limitlessled-rgb.rb)
Now you can control your LimitlessLED lights on Android using AutoVoice and Tasker/Locale.
LimitlessLED LightShow made in Perl programming language by Prof. Matt Carter from Bond University

[https://github.com/hash-bang/Lightshow](https://github.com/hash-bang/Lightshow)

provides a simple command line program to command the lighting system as well as also supports easy to program lighting macros - fake fire place lighting, time-of-day settings etc.

Matt is also working on an XBMC/BoxEE plugin for media centers so that you can control the lights from your TV.  Great work Matt, look forward to this one.
Jon Benson from Australia has built this great Tasker app.
Tasker stays in memory when launched once, so it displays almost instantly.

The current app (which simply allows sending UDP packets by acting as a plugin for Tasker/Locale) is free and will remain so.
Here is the Github webpage:

[http://hastarin.github.io/android-udpsender/](http://hastarin.github.io/android-udpsender/)
and here is the Play Store link:

[https://play.google.com/store/apps/details?id=com.hastarin.android.udpsender](https://play.google.com/store/apps/details?id=com.hastarin.android.udpsender)
To give you an idea of how it can be used, Jon has set this up so far...

* Gesture control via my launcher (Nova) to turn all lights on/off/full bright/night mode and dim/brighten lights.

* Using the Scene shown above I can turn all/individual zones on/off and control bright/dim/warm/cool.

* NOTE: A swipe up/down in the colored square controls a bright/dim ramp and left/right for warm/cool.

* An NFC tag on my bed that when tapped will turn the lights off, or keep some in night mode if I have guests.

* When my phone connects to the Wi-Fi, and it's between sunrise and sunset, my entry hall light is turned on.