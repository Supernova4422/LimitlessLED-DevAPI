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



## Microsoft C#.NET example Code

        //Connect to LimitlessLED Wifi Bridge Receiver
	System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient("255.255.255.255", 8899);

	//Send hex command 38 which is "Turn Group1 LED lights ON" yes it remembers the last brightness and color, each LED contains a memory chip.
	udpClient.Send(new byte[] {
		0x38,
		0x0,
		0x55
	}, 3);
	//ToDo: send as many different commands here as you like, just change the number above where you see &#038;H38

	//Close Connection
	udpClient.Close();


## Microsoft C#.NET Windows Phone 7.1/8.0/RT example Code
Example code thanks to VikingCode. WP7.1/8/WinRT don't have System.IO.Sockets, instead they make use of a 'datagram socket', and the code for that looks like this (tested, works on my RGB light) WinRT requires Private Networks (Client &#038; Server) to be selected.


    public async Task TurnOnRGBLight()
            {
                var socket = new DatagramSocket();
                using (var stream = await socket.GetOutputStreamAsync(new HostName("192.168.1.255"), "8899"))
                {
                    using (var writer = new DataWriter(stream))
                    {
                        writer.WriteBytes(new byte[] { 0x22, 0x0, 0x55 });
                        writer.StoreAsync();
                    }
                }
            }
