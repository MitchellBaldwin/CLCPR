/*
 Name:		CLCPRDevice.ino
 Created:	1/5/2016 7:40:44 PM
 Author:	Mitchell Baldwin
*/
#include <PacketSerial.h>

#define LEDPIN 13					
#define PACKET_SIZE 30
uint8_t inPacket[PACKET_SIZE];
uint8_t outPacket[PACKET_SIZE];

PacketSerial spUSB;

void setup()
{
	pinMode(LEDPIN, OUTPUT);

	for (int i = 0; i < PACKET_SIZE; ++i)
	{
		inPacket[i] = 0x00;
		outPacket[i] = 0x00;
	}
	spUSB.setPacketHandler(&OnUSBPacket);
	spUSB.begin(115200);

}

void loop()
{
	spUSB.update();
	delay(10);

}

void OnUSBPacket(const uint8_t* buffer, size_t size)
{
	if (buffer[0] == 0x10)				// Test case
	{
		ToggleUserLED();
		outPacket[0] = buffer[0] + 1;
		outPacket[1] = buffer[1] + 1;
		outPacket[2] = buffer[2] + 1;
		outPacket[3] = buffer[3] + 1;

		int checksum = 0;
		for (int i = 0; i < PACKET_SIZE - 1; ++i)
		{
			checksum += outPacket[i];
		}
		outPacket[PACKET_SIZE - 1] = checksum;
		spUSB.send(outPacket, PACKET_SIZE);
	}
}

void ToggleUserLED()
{
	if (digitalRead(LEDPIN) == HIGH)
	{
		digitalWrite(LEDPIN, LOW);
	}
	else
	{
		digitalWrite(LEDPIN, HIGH);
	}
}
