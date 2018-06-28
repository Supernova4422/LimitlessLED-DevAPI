
### LAST UPDATED: 29th January 2017

# Domoticz wifi bridge v6 Code

[https://github.com/domoticz/domoticz](https://github.com/domoticz/domoticz)
The sample code for wifi bridge v6 is here:
[https://github.com/domoticz/domoticz/blob/master/hardware/Limitless.cpp](https://github.com/domoticz/domoticz/blob/master/hardware/Limitless.cpp)


# LimitlessLED Wifi Bridge v6.0


## Wifi Bridge v6 Protocol - All values are in Hex

all values below are in hex, 0xFF  means 255 in decimal aka FF in hex, aka 1 byte of FF, aka 8 bits of 11111111.

    UDP.IP = "255.255.255.255"; (or direct Wifi Bridge IP Address)
    UDP.PORT = 5987; //(decimal integer 5987)
    UDP.SEND (hex bytes, see below)

SN = Sequential Byte
Sequential byte just helps with keeping commands in the correct order, and it helps to ignore duplicate packets already received. increment this byte for each new command by 1.

    WB1 = LimitlessLED Wifi Bridge Session ID1
    WB2 = LimitlessLED Wifi Bridge Session ID2
    to get the WB1 and WB2 send this command
    UDP.SEND hex bytes: 20 00 00 00 16 02 62 3A D5 ED A3 01 AE 08 2D 46 61 41 A7 F6 DC AF (D3 E6) 00 00 1E  <-- Send this to the ip address of the wifi bridge v6
    UDP.Response: 28 00 00 00 11 00 02 (AC CF 23 F5 7A D4)(mac) 69 F0 3C 23 00 01 (05 00)(<--WifiBridgeSessionID1 &#038; WifiBridgeSessionID2) 00 <-- (example response 1)
    UDP.Response: 28 00 00 00 11 00 02 (AC CF 23 F5 7A D4)(mac) 69 F0 3C 23 00 01 (17 01)(<--WifiBridgeSessionID1 &#038; WifiBridgeSessionID2) 00 <-- (example response 2)
    UDP.Response: 28 00 00 00 11 00 02 (AC CF 23 F5 7A D4)(mac) 69 F0 3C 23 00 01 (F2 00)(<--WifiBridgeSessionID1 &#038; WifiBridgeSessionID2) 00 <-- (example response 3)
    WB1 = LimitlessLED Wifi Bridge Session ID1  = response[19] (20th byte of response above)
    WB2 = LimitlessLED Wifi Bridge Session ID2  = response[20] (21st byte of response above)

<b>RGBW/WW/CW Commands</b>

    UDP Hex Send Format: 80 00 00 00 11 {WifiBridgeSessionID1} {WifiBridgeSessionID2} 00 {SequenceNumber} 00 {COMMAND} {ZONE NUMBER} 00 {Checksum}
    UDP Hex Response: 88 00 00 00 03 00 {SequenceNumber} 00

<b>List of {COMMAND}s:</b>
format of {command} 9 byte packet = 0x31 {PasswordByte1 default 00} {PasswordByte2 default 00} {remoteStyle 08 for RGBW/WW/CW or 00 for bridge lamp} {LightCommandByte1} {LightCommandByte2} 0x00 0x00 0x00 {Zone1-4 0=All} 0x00 {Checksum}

    31 00 00 08 04 01 00 00 00 = Light ON
    31 00 00 08 04 02 00 00 00 = Light OFF
    31 00 00 08 04 05 00 00 00 = Night Light ON
    31 00 00 08 05 64 00 00 00 = White Light ON (Color RGB OFF)
    31 00 00 08 01 BA BA BA BA = Set Color to Blue (0xBA) (0xFF = Red, D9 = Lavender, BA = Blue, 85 = Aqua, 7A = Green, 54 = Lime, 3B = Yellow, 1E = Orange)
    31 00 00 08 02 SS 00 00 00 = Saturation (SS hex values 0x00 to 0x64 : examples: 00 = 0%, 19 = 25%, 32 = 50%, 4B, = 75%, 64 = 100%)
    31 00 00 08 03 BN 00 00 00 = BrightNess (BN hex values 0x00 to 0x64 : examples: 00 = 0%, 19 = 25%, 32 = 50%, 4B, = 75%, 64 = 100%)
    31 00 00 08 05 KV 00 00 00 = Kelvin (KV hex values 0x00 to 0x64 : examples: 00 = 2700K (Warm White), 19 = 3650K, 32 = 4600K, 4B, = 5550K, 64 = 6500K (Cool White))
    31 00 00 08 06 MO 00 00 00 = Mode Number (MO hex values 0x01 to 0x09 : examples: 01 = Mode1, 02 = Mode2, 03 = Mode3 .. 09 = Mode9)
    31 00 00 08 04 04 00 00 00 = Mode Speed Decrease--
    31 00 00 08 04 03 00 00 00 = Mode Speed Increase++
    3D 00 00 08 00 00 00 00 00 = Link (Sync Bulb within 3 seconds of lightbulb socket power on)
    3E 00 00 08 00 00 00 00 00 = UnLink (Clear Bulb within 3 seconds of lightbulb socket power on)

Wifi Bridge iBox LED Lamp {COMMAND}s (Zone Number = 0x01)

    31 00 00 00 03 03 00 00 00 = Wifi Bridge Lamp ON
    31 00 00 00 03 04 00 00 00 = Wifi Bridge Lamp OFF
    31 00 00 00 04 MO 00 00 00 = Mode Number (MO hex values 0x01 to 0x09 : examples: 01 = Mode1, 02 = Mode2, 03 = Mode3 .. 09 = Mode9)
    31 00 00 00 03 01 00 00 00 = Mode Speed Decrease--
    31 00 00 00 03 02 00 00 00 = Mode Speed Increase++
    31 00 00 00 01 BA BA BA BA = Set Color to Blue (BA) (FF = Red, D9 = Lavender, BA = Blue, 85 = Aqua, 7A = Green, 54 = Lime, 3B = Yellow, 1E = Orange)
    31 00 00 00 03 05 00 00 00 = Set Color to White (is ignored when Lamp is OFF, it does NOT turn the Lamp ON)
    31 00 00 00 02 BN 00 00 00 = BrightNess (BN hex values 0x00 to 0x64 : examples: 00 = 0%, 19 = 25%, 32 = 50%, 4B, = 75%, 64 = 100%)

<b>Valid List for {ZONE NUMBER}</b>

    0x00 All
    0x01 Zone1
    0x02 Zone2
    0x03 Zone3
    0x04 Zone4

<b>Checksum:</b>
RGBW/WW/CW Checksum Byte Calculation is the sum of the 11 bytes before end of the UDP packet. The checksum is then added to the end of the UDP message.
take the 9 bytes of the command, and 1 byte of the zone, and add the 0 = the checksum = (checksum &#038; 0xFF)
e.g. SUM((31 00 00 08 04 01 00 00 00)(command) 01(zone) 00) = 3F(chksum)

<b>EXAMPLES</b>

    LimitlessLED Wifi Bridge Light ON   80 00 00 00 11 WB 00 00 SN 00 (31 00 00 00 03 03 00 00 00)(cmd) 01(zone) 00 38(chksum)  UDP response: (88 00 00 00 03 00 SN 00)
    LimitlessLED Wifi Bridge Light OFF  80 00 00 00 11 WB 00 00 SN 00 (31 00 00 00 03 04 00 00 00)(cmd) 01(zone) 00 39(chksum)  UDP response: (88 00 00 00 03 00 SN 00)

    RGBW/WW/CW Zone 1 ON  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 01 00 00 00)(cmd) 01(zone) 00 3F(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone1 OFF  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 02 00 00 00)(cmd) 01(zone) 00 40(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 2 ON  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 01 00 00 00)(cmd) 02(zone) 00 40(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone2 OFF  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 02 00 00 00)(cmd) 02(zone) 00 41(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 3 ON  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 01 00 00 00)(cmd) 03(zone) 00 41(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone3 OFF  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 02 00 00 00)(cmd) 03(zone) 00 42(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 4 ON  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 01 00 00 00)(cmd) 04(zone) 00 42(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone4 OFF  80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 02 00 00 00)(cmd) 04(zone) 00 43(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW ZoneALL ON 80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 01 00 00 00)(cmd) 00(zone) 00 3E(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW ZoneALLOFF 80 00 00 00 11(length hex) (17 01)(WB1WB2) 00 SN 00 (31 00 00 08 04 02 00 00 00)(cmd) 00(zone) 00 3F(chksum)  response: (88 00 00 00 03 00 SN 00)

    RGBW/WW/CW Zone 1 Link 80 00 00 00 11 WB1 WB2 00 SN 00 (3D 00 00 08 00 00 00 00 00)(link cmd) 01(zone) 00 46(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 2 Link 80 00 00 00 11 WB1 WB2 00 SN 00 (3D 00 00 08 00 00 00 00 00)(link cmd) 02(zone) 00 47(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 3 Link 80 00 00 00 11 WB1 WB2 00 SN 00 (3D 00 00 08 00 00 00 00 00)(link cmd) 03(zone) 00 48(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 4 Link 80 00 00 00 11 WB1 WB2 00 SN 00 (3D 00 00 08 00 00 00 00 00)(link cmd) 04(zone) 00 49(chksum)  response: (88 00 00 00 03 00 SN 00)

    RGBW/WW/CW Zone 1 UnLink 80 00 00 00 11 WB 00 00 SN 00 (3E 00 00 08 00 00 00 00 00)(unlink cmd) 01(zone) 00 47(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 2 UnLink 80 00 00 00 11 WB 00 00 SN 00 (3E 00 00 08 00 00 00 00 00)(unlink cmd) 02(zone) 00 48(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 3 UnLink 80 00 00 00 11 WB 00 00 SN 00 (3E 00 00 08 00 00 00 00 00)(unlink cmd) 03(zone) 00 49(chksum)  response: (88 00 00 00 03 00 SN 00)
    RGBW/WW/CW Zone 4 UnLink 80 00 00 00 11 WB 00 00 SN 00 (3E 00 00 08 00 00 00 00 00)(unlink cmd) 04(zone) 00 4A(chksum)  response: (88 00 00 00 03 00 SN 00)

<b>Keep Alive Messages</b>

    KEEP ALIVES (Every 5 seconds) Wifi Bridge 1: D0 00 00 00 02 (WB) 00  (response: D8 00 00 00 07 (AC CF 23 F5 7A D4) 01)
    KEEP ALIVES (Every 5 seconds) Wifi Bridge 1: D0 00 00 00 02 1D 00  (response: D8 00 00 00 07 (AC CF 23 F5 7A D4) 01)
    KEEP ALIVES (Every 5 seconds) Wifi Bridge 2: D0 00 00 00 02 7C 00  (response: D8 00 00 00 07 (AC CF 23 F5 7D 80) 01)

<b>Click Search for Devices: </b>

    UDP.Send (255.255.255.255:5987) Bytes: 10 00 00 00 24 02 ee 3e 02 39 38 35 62 31 35 37 62 66 36 66 63 34 33 33 36 38 61 36 33 34 36 37 65 61 33 62 31 39 64 30 64

<b>GET ALL WIFI BRIDGE CLOUD KEYS on LAN using UDP</b>
   
    UDP.IP = "255.255.255.255"
    UDP.port = 5987
    UDP.SEND hex bytes: 10 00 00 00 0A 02 D3 E6 01 (AC CF 23 F5 7A D4)(MAC address)
    UDP.Response: 18 00 00 00 40 02 (AC CF 23 F5 7A D4)(mac) 00 20 (985b157bf6fc43368a63467ea3b19d0d)(ASCII Tokenkey) 01 00 01 17 63 00 00 05 00 09 (xlink_dev)(ASCII) 07 5B CD 15
    UDP.SEND hex bytes: 20 00 00 00 16 02 62 3A D5 ED A3 01 AE 08 2D 46 61 41 A7 F6 DC AF D3 E6 00 00 1E
    UDP.Response: 28 00 00 00 11(LENGTH) 00 02 (AC CF 23 F5 7A D4)(mac) 69 F0 3C 23 00 01 05 00 00
    UDP.SEND hex bytes: D0 00 00 00 02 05(WB1) 00(WB2)
    UDP.Response: response: D8 00 00 00 07 (AC CF 23 F5 7A D4) 01)
    example2: 10 00 00 00 0A 02 FE E7 01 (AC CF 23 F5 7A D4)(MAC address)
    example3: 10 00 00 00 0A 02 FE 51 01 (AC CF 23 F5 7D 80)(MAC address)

## Section 4. Smart Link Setup Services
* ** Warning only have one wifi bridge powered up at a time when setup using smart link (In case you have more than one in your home) **
* Smart-link is all about getting your brand new wifi bridge onto your home wifi router network.
* SmartLink saves the user time during the setup phase, from having to switch to the bridge in the wifi settings to set it up. Instead it is done using a sync button underneath the Wifi Bridge v6 and an app.
* If you want to build smart-link into your own app.
* SMART LINK UDP port 49999  Default Password "8888"
* UDP.IP = "10.1.1.255"
* UDP.PORT = 49999
* UDP.Send Whilst Waiting:  05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05

## Section 5. Cloud Remote Internet access Services
* If you want to build an app that controls the lights over the internet, more information will be made available here.
* [http://www.xlink.cn/](http://www.xlink.cn/)
* So that enterprises have the power of things. Cloud-to-cloud platform, easily and securely connected devices, rapid development of Internet of things applications,

In the Internet of Things data extraction value.
* Default Cloud Server: Amazon Web Services Cloud ec2-52-63-118-215.ap-southeast-2.compute.amazonaws.com [52.63.118.215] cm2.xlink.cn
* Default Cloud Port: 23778
* Cloud Server Login TCP.Send: 10 00 00 00 1A 02 4B 4B 59 DB 00 10 32 32 30 66 61 32 61 66 66 65 36 63 61 32 30 30 00 00 3C

* Cloud TCP.Read Login Response: 18 00 00 00 02 00 00
* Cloud Keep Alive: TCP.Send: D0 00 00 00 00
* Cloud Keep Alive: TCP.Read: D8 00 00 00 00
* Cloud TCP sent: A0 00 00 00 07 69 F0 3D F3 00 0B 00
* Cloud response: A8 00 00 00 13 69 F0 3D F3 00 0B 00 07 00 09 x l i n k _ d e v
* Cloud TCP sent:
* Cloud response: 6{"device_id":1777352179,"type":"offline","operator":0}


#### RGB+W LimitlessLED Light Bulb Commands



All UDP Commands are 3 Bytes. First byte is from the list below, plus a fixed 2 byte suffix of 0x00 (decimal: 0) and 0x55 (decimal: 85)

i.e. to turn all RGBW COLOR LimitlessLED Smart lights to ON then send the TCP/IP UDP packet of:  0x42 0x00 0x55

                    Hexidecimal (byte)	 Decimal (integer)

    RGBW COLOR LED ALL OFF	   0x41	           65

    RGBW COLOR LED ALL ON	   0x42	           66

    DISCO SPEED SLOWER	   0x43	           67

    DISCO SPEED FASTER	   0x44	           68

    GROUP 1 ALL ON		   0x45	           69	(SYNC/PAIR RGB+W Bulb within 2 seconds of Wall Switch Power being turned ON)

    GROUP 1 ALL OFF		   0x46	           70

    GROUP 2 ALL ON		   0x47	           71	(SYNC/PAIR RGB+W Bulb within 2 seconds of Wall Switch Power being turned ON)

    GROUP 2 ALL OFF		   0x48	           72

    GROUP 3 ALL ON		   0x49	           73	(SYNC/PAIR RGB+W Bulb within 2 seconds of Wall Switch Power being turned ON)

    GROUP 3 ALL OFF		   0x4A	           74

    GROUP 4 ALL ON		   0x4B	           75	(SYNC/PAIR RGB+W Bulb within 2 seconds of Wall Switch Power being turned ON)

    GROUP 4 ALL OFF		   0x4C	           76

    DISCO MODE	           0x4D	           77

    SET COLOR TO WHITE (GROUP ALL)  0x42    100ms followed by:	0xC2

    SET COLOR TO WHITE (GROUP 1)    0x45	100ms followed by:	0xC5

    SET COLOR TO WHITE (GROUP 2)    0x47	100ms followed by:	0xC7

    SET COLOR TO WHITE (GROUP 3)    0x49	100ms followed by:	0xC9

    SET COLOR TO WHITE (GROUP 4)    0x4B	100ms followed by:	0xCB



LIMITLESSLED RGBW DIRECTLY SETTING THE BRIGHTNESS is by a 3BYTE COMMAND: (First send the Group ON for the group you want to set the brightness for. You send the group ON command 100ms before sending the 4E 1B 55)

    Byte1: 0x4E        (decimal: 78)

    Byte2: 0x02 to 0x1B (decimal range: 2 to 27) full brightness 0x1B (decimal 27)

    Byte3: Always 0x55 (decimal: 85)
    LIMITLESSLED RGBW COLOR SETTING is by a 3BYTE COMMAND: (First send the Group ON for the group you want to set the colour for. You send the group ON command 100ms before sending the 40)

    Byte1: 0x40        (decimal: 64)

    Byte2: 0x00 to 0xFF     (255 colors)   See Color Matrix Chart for the different values below.

    Byte3: Always 0x55 (decimal: 85)

Byte2: Color Matrix Chart: (thanks Stephan Schaade, http://knx-user-forum.de http://mknx.github.io/smarthome/)

note there are more colours (0-255) in between, this color chart is just steps of 16.
    
    0x00 Violet
    0x10 Royal_Blue
    0x20 Baby_Blue
    0x30 Aqua
    0x40 Mint
    0x50 Seafoam_Green
    0x60 Green
    0x70 Lime_Green
    0x80 Yellow
    0x90 Yellow_Orange
    0xA0 Orange
    0xB0 Red
    0xC0 Pink
    0xD0 Fusia
    0xE0 Lilac
    0xF0 Lavendar

Thanks Andrew Badge, here is another copy of the color chart, The last column (HTML colour code) is used for rendering onscreen previews.
Note: the names are modified to match the websafe names as well.

            table.Rows.Add("Violet",0x00,"#EE82EE");
            table.Rows.Add("RoyalBlue",0x10,"#4169E1");
            table.Rows.Add("LightSkyBlue", 0x20,"#87CEFA");
            table.Rows.Add("Aqua",0x30,"#00FFFF");
            table.Rows.Add("AquaMarine", 0x40,"#7FFFD4");
            table.Rows.Add("SeaGreen",0x50,"#2E8B57");
            table.Rows.Add("Green",0x60,"#008000");
            table.Rows.Add("LimeGreen",0x70,"#32CD32");
            table.Rows.Add("Yellow",0x80,"#FFFF00");
            table.Rows.Add("Goldenrod",0x90,"#DAA520");
            table.Rows.Add("Orange",0xA0,"#FFA500");
            table.Rows.Add("Red",0xB0,"#FF0000");
            table.Rows.Add("Pink",0xC0,"#FFC0CB");
            table.Rows.Add("Fuchsia", 0xD0,"#FF00FF");
            table.Rows.Add("Orchid",0xE0,"#DA70D6");
            table.Rows.Add("Lavender",0xF0,"#E6E6FA");



Night modes, not implemented for RGBW, but referenced here for completeness..

    NIGHT MODE ALL		        0x41	100ms followed by:	0xC1   (not implemented in limitlessled wifi bridge 3.0)

    NIGHT SAVER MODE GROUP 1	0x46	100ms followed by:	0xC6   (not implemented in limitlessled wifi bridge 3.0)

    NIGHT SAVER MODE GROUP 2	0x48	100ms followed by:	0xC8   (not implemented in limitlessled wifi bridge 3.0)

    NIGHT SAVER MODE GROUP 3	0x4A	100ms followed by:	0xCA   (not implemented in limitlessled wifi bridge 3.0)

    NIGHT SAVER MODE GROUP 4	0x4C	100ms followed by:	0xCC   (not implemented in limitlessled wifi bridge 3.0)
Special NOTE #1: The LimitlessLED bulb remembers its own Brightness setting memory separately for ColorRGB and separately for White. For example dimming Green, then switching to White full brightness, and when you go back to a specific color the brightness returns to what the ColorRGB was before. Same vice versa for the White. The previous brightness setting is remembered specifically for the White and specifically for the ColorRGB.

Special Note #2: for Java:  To convert from a decimal number to a signed byte use the 0xFF like this:  cmdBytes[0] = 85 &#038; 0xFF;

Special Note #3: for Visual Basic: To use a hexadecimal number, you can put &#038;H in front instead of 0x  for example: 0x41 would be &#038;H41

Special Note #4: If you don't know the IP address of the Wifi Bridge, You can also broadcast All Groups ON to all wifi bridges on the LAN network by using the IP address with the last number as .255  or you can use the IP address of 255.255.255.255, which will do a full LAN broadcast to all wifi bridges on this Local network only.

Special Note #5: With the new LimitlessLED Wifi Bridge 3.0  it is no longer necessary to send the 3rd byte (0x55) in the UDP packet, you can just send the 1st and 2nd bytes which will be faster, this also applies to the backward compatible RGB and DUAL White LEDs when using wifi bridge 3.0.  But to remain backwards compatible with wifi bridge 2.0 it is advisable to always send the 3rd byte 0x55 in the UDP packet for RGB and WHITE bulbs, but it is not necessary to send 3 bytes for the new RGBW bulbs(just two is ok for them).

Special Note #6: To Clear the PAIRED/SYNCED bulb from all of its Groups.. Press-And-Hold its Group ON button within 2 seconds of turning the light socket power ON.
## LimitlessLED v4.0 Wifi Bridge Control Commandset

==================================================================

==== LimitlessLED WIFI Bridge Auto Discovery ADMIN PORT 48899 ====

==================================================================

- Step 1:Send UDP message to the LAN broadcast IP address of "10.10.100.255" and port of 48899 => "Link_Wi-Fi"

- All Wifi bridges on the LAN will respond with their details. Response is "10.10.100.254, ACCF232483E8"
- Step 2 (optional for changing settings on the wifi bridge): Then send "+ok" to the LimitlessLED Wifi Bridge. Send UDP message to the response IP address returned from step 1 "10.10.100.254" => "+ok"

- Step 3 (optional for changing settings on the wifi bridge): After that you may send AT commands (ending with \r\n) to the module.


The first time the bridge is set-up you connect the phone directly to the bridge's wifi, then send the WSCAN command to find either the wifi's that the bridge can detect? If you already know the Wifi SSID, you can skip that step and just use the WSSSID command.  replace mySSIDname with the SSID you want to use.  replace password123 with your wifi password.
The LimitlessLED Wifi Bridge consists of a Wifi to Serial bridge chip LPT-100 and includes a Custom MCU designed and built to spec by Lierda, and also contains a PL1167 Low Power High Performance Single Chip 2.4GHz Transceiver used as the transmitter in the wifi bridge and used in the receiver of each bulb. The communications are secured over the air using encryption and frequency hopping.


## LimitlessLED RGB CommandSet (RGB bulbs no longer sold Jan 2014)
Terms of Use: All content is copyright 2013 Limitless Designs LLC, Under no circimstances is this information to be reproduced, copied, nor distributed without written permission.

Warning: reproduction of this information without written confirmation is an offense, and is strictly enforced.

If this information is used in your programming files then you must reference the limitlessLED dev webpage using the following Text:
// Source: http://www.limitlessled.com/dev

// Copyright (2013) Limitless Designs LLC

// If these codes are copied or utilized in any way, the LimitlessLED website link must remain attached.

# DETAILED DEVELOPER TECHNICAL SPECIFICATIONS

Connect iPhone/iPad direct to WiFi Bridge Receiver: Yes (WiFi adhoc mode)

Connect WiFi Bridge Receiver to LAN: Yes (WiFi infrastructure mode)
IP Address: 192.168.1.100 (editable)   or ip 10.10.100.255 AP, or 10.1.1.255 when on STA connected to your lan.

Port: 50000 (editable)  or port 8899 since bridge v3.0

TCP/IP Mode: UDP (udp is what online games use, it is very fast and has the lowest latency)

Username: admin (editable)

Password: 000000 (editable)
<strong>RGB COLOR LimitlessLED WIFI TCP/IP UDP COMMANDS</strong>

All UDP Commands are 3 Bytes. First byte is from the list below, plus a fixed 2 byte suffix of 0x00 (decimal: 0) and 0x55 (decimal: 85)

i.e. to turn all RGB COLOR LimitlessLED Smart lights to ON then send the TCP/IP UDP packet of:  0x22 0x00 0x55



    Command             Hexidecimal (byte)   Decimal (integer)

    RGB COLOR LED ALL OFF	   0x21	           33

    RGB COLOR LED ALL ON	   0x22	           34

    BRIGHTNESS UP	           0x23	           35

    BRIGHTNESS DOWN	           0x24	           36

    DISCO SPEED FASTER	   0x25	           37    (SYNC/PAIR RGB Bulb within 2 seconds of Wall Switch Power being turned ON)

    DISCO SPEED SLOWER	   0x26	           38

    DISCO MODE NEXT	           0x27	           39

    DISCO MODE PREVIOUS	   0x28	           40



    COLOR SETTING is by a 3BYTE COMMAND:

    Byte1: 0x20

    Byte2: 0x00 to 0xFF

    Byte3: Always 0x55 (decimal: 85)
<strong>WARM WHITE/COOL WHITE LimitlessLED Smartbulb WIFI TCP/IP UDP COMMANDS</strong>




	Hexidecimal (byte)	Decimal (integer)

    WHITE LED ALL OFF	0x39	57

    WHITE LED ALL ON	0x35	53

    BRIGHTNESS UP		0x3C	60

    BRIGHTNESS DOWN		0x34	52

    WARM WHITE INCREASE	0x3E	62

    COOL WHITE INCREASE	0x3F	63

    GROUP 1 ALL ON		0x38	56

    GROUP 1 ALL OFF		0x3B	59

    GROUP 2 ALL ON		0x3D	61

    GROUP 2 ALL OFF		0x33	51

    GROUP 3 ALL ON		0x37	55

    GROUP 3 ALL OFF		0x3A	58

    GROUP 4 ALL ON		0x32	50

    GROUP 4 ALL OFF		0x36	54

    NIGHT MODE ALL		        0x39	100ms followed by:	0xB9

    NIGHT SAVER MODE GROUP 1	0x3B	100ms followed by:	0xBB

    NIGHT SAVER MODE GROUP 2	0x33	100ms followed by:	0xB3

    NIGHT SAVER MODE GROUP 3	0x3A	100ms followed by:	0xBA

    NIGHT SAVER MODE GROUP 4	0x36	100ms followed by:	0xB6

    FULL BRIGHTNESS ALL		0x35	100ms followed by:	0xB5

    FULL BRIGHTNESS GROUP 1		0x38	100ms followed by:	0xB8

    FULL BRIGHTNESS GROUP 2		0x3D	100ms followed by:	0xBD

    FULL BRIGHTNESS GROUP 3		0x37	100ms followed by:	0xB7

    FULL BRIGHTNESS GROUP 4		0x32	100ms followed by:	0xB2





## Summary of Codes (send all 3 hex commands via port 8899 UDP to ip address of the wifi bridge receiver)
LimitlessLED White


    35 00 55 - All On

    39 00 55 - All Off

    3C 00 55 - Brightness Up

    34 00 55 - Brightness Down  (There are ten steps between min and max)

    3E 00 55 - Warmer

    3F 00 55 - Cooler  (There are ten steps between warmest and coolest)

    38 00 55 - Zone 1 On

    3B 00 55 - Zone 1 Off

    3D 00 55 - Zone 2 On

    33 00 55 - Zone 2 Off

    37 00 55 - Zone 3 On

    3A 00 55 - Zone 3 Off

    32 00 55 - Zone 4 On

    36 00 55 - Zone 4 Off

    B5 00 55 - All On Full    (Send >=100ms after All On)

    B8 00 55 - Zone 1 Full  (Send >=100ms after Zone 1 On)

    BD 00 55 - Zone 2 Full  (Send >=100ms after Zone 2 On)

    B7 00 55 - Zone 3 Full  (Send >=100ms after Zone 3 On)

    B2 00 55 - Zone 4 Full  (Send >=100ms after Zone 4 On)

    B9 00 55 - All Nightlight         (Send >=100ms after All Off)

    BB 00 55 - Zone 1 Nightlight  (Send >=100ms after Zone 1 Off)

    B3 00 55 - Zone 2 Nightlight  (Send >=100ms after Zone 2 Off)

    BA 00 55 - Zone 3 Nightlight  (Send >=100ms after Zone 3 Off)

    B6 00 55 - Zone 4 Nightlight  (Send >=100ms after Zone 4 Off)
LimitlessLED RGB

    22 00 55 - Lamps On

    21 00 55 - Lamps Off

    23 00 55 - Brightness Up

    24 00 55 - Brightness Down  (There are nine steps between min and max)

    27 00 55 - Mode Up

    28 00 55 - Mode Down  (There are 20 modes. The lowest is constant white)

    25 00 55 - Speed Up (Fast)

    26 00 55 - Speed Down (Slow)

    20 xx 55 - Set Colour to xx 
LimitlessLED RGBW

 

    41 00 55 - All Off

    42 00 55 - All On

    43 00 55 - Speed Down (One Step Slower Disco)

    44 00 55 - Speed Up (One Step Faster Disco)

    45 00 55 - Zone 1 On

    46 00 55 - Zone 1 Off

    47 00 55 - Zone 2 On

    48 00 55 - Zone 2 Off

    49 00 55 - Zone 3 On

    4A 00 55 - Zone 3 Off

    4B 00 55 - Zone 4 On

    4C 00 55 - Zone 4 Off

    4D 00 55 - One Step Disco Mode Up (20 Disco Modes)

    42 00 55 wait 100ms then send C2 00 55 - All Zones Change back to Warm White.

    45 00 55 wait 100ms then send C5 00 55 - Zone 1 Change back to Warm White.

    47 00 55 wait 100ms then send C7 00 55 - Zone 2 Change back to Warm White.

    49 00 55 wait 100ms then send C9 00 55 - Zone 3 Change back to Warm White.

    4B 00 55 wait 100ms then send CB 00 55 - Zone 4 Change back to Warm White.

    42 00 55 wait 100ms then send 4E XX 55 - Set All to Brightness XX (XX range is 0x02 to 0x1B)

    45 00 55 wait 100ms then send 4E XX 55 - Set Zone 1 to Brightness XX (XX range is 0x02 to 0x1B)

    47 00 55 wait 100ms then send 4E XX 55 - Set Zone 2 to Brightness XX (XX range is 0x02 to 0x1B)

    49 00 55 wait 100ms then send 4E XX 55 - Set Zone 3 to Brightness XX (XX range is 0x02 to 0x1B)

    4B 00 55 wait 100ms then send 4E XX 55 - Set Zone 4 to Brightness XX (XX range is 0x02 to 0x1B)

    42 00 55 wait 100ms then send 40 XX 55 - Set All to Color XX (XX range is 0x00 to 0xFF)

    45 00 55 wait 100ms then send 40 XX 55 - Set Zone 1 to Color XX (XX range is 0x00 to 0xFF)

    47 00 55 wait 100ms then send 40 XX 55 - Set Zone 2 to Color XX (XX range is 0x00 to 0xFF)

    49 00 55 wait 100ms then send 40 XX 55 - Set Zone 3 to Color XX (XX range is 0x00 to 0xFF)

    4B 00 55 wait 100ms then send 40 XX 55 - Set Zone 4 to Color XX (XX range is 0x00 to 0xFF)

## SPI COMMANDSET 2.4Ghz RF Over-the-Air Protocol - Remote Control Commands using PL1176 (or compatible LT9800)

Control individual bulbs directly after you have synced them to the remoteID.

PL1176 2.4Ghz Transceiver Settings: 

    Register20: syncword 32 bits, trailer 4 bits, data type NRZ, FEC None      (0x48 0x00) 
    Register29: CRC on, first byte is length, FW_TERM_TX, initial CRC data 00  (0xB0 0x00)

Broadcast remote command over the Air:

    Register07: Channel 9 2411MHz (0x01 0x09)
    Preamble = ON (0xAAAAAA) 3 bytes
    SyncBytes = ON (0x147A, 0x258B) 2 bytes
    Trailer = ON (0x05) 4 bits little endian (1010)
    Length = (0x07) 1 byte
    CommandType = 0xB0 (color, white) or 0xB8 (group/all on/off) 1 byte
    RemoteID = (2 bytes) (this is securely stored into the bulb during remote-bulb sync at power on, max of 4 per bulb, syncing a 5th RemoteID drops the first one that is stored in that bulb. When clearing the bulb it removes all 4 remote ids from the bulb)
    Color = (0x00 to 0xFF) See colour chart above. 1 byte
    Brightness = (0x00 to 0xFF) See brightness chart above.  1 byte
    Command = (0x01 to 0x1A) See command list below. 1 byte
    Checksum = See checksum calc function code below. 2 Bytes
    Repeat again using Channel 40 2442MHz (Set Register 7: 0x01 0x28)
    Repeat again using Channel 71 2473MHz (Set Register 7: 0x01 0x47)
    Repeat again 5 to 40 times to ensure the bulbs receive the command. If sent a lower number of times, increase the time between sends.

Available Commands

    0x01, // All ON
    0x02, // All OFF
    0x03, // Group 1 ON
    0x04, // Group 1 OFF
    0x05, // Group 2 ON
    0x06, // Group 2 OFF
    0x07, // Group 3 ON
    0x08, // Group 3 OFF
    0x09, // Group 4 ON
    0x0A, // Group 4 OFF
    0x0B, // Disco Speed Increase
    0x0C, // Disco Speed Decrease
    0x0D, // Disco Mode
    0x11, // Set Color White - All Groups
    0x13, // Set Color White - Group 1
    0x15, // Set Color White - Group 2
    0x17, // Set Color White - Group 3
    0x19, // Set Color White - Group 4
    0x12, // Night Mode - All Groups
    0x14, // Night Mode - Group 1
    0x16, // Night Mode - Group 2
    0x18, // Night Mode - Group 3
    0x1A  // Night Mode - Group 4

//Thanks to Henryk and Erantimus for providing details and checksum code.
//Calculate Checksum - Returns 2 bytes.

    uint16 calc_crc(uint8 data[], data_length = 0x08) {
    uint16 state = 0;
    for (size i = 0; i < data_length; i++) {
        uint8 byte = data[i];
        for (int j = 0; j < 8; j++) {
        if ((byte ^ state) &#038; 0x01) {
            state = (state >> 1) ^ 0x8408;
        } else {
            state = state >> 1;
        }
        byte = byte >> 1;
        }
    }
    return state;
    }



