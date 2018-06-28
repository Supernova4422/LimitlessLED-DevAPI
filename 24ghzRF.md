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
