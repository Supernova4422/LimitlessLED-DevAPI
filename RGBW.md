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