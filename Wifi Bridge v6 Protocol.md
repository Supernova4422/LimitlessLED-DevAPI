## Section 3. Wifi Bridge v6 Protocol - All values are in Hex

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
