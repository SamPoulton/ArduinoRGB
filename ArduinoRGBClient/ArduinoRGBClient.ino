#include <String.h>
const int BAUD_RATE = 115200;
const char DEVICE_NAME[] = "Test Device";
const char ENDPOINT_NAME[] = "Test Header";

const int RED_PIN = 9;
const int GREEN_PIN = 10;
const int BLUE_PIN = 11;

String buffer = "";

bool isGradient = false;

bool gradientDirection = false; // true means hue is going from 1 to 2


int red, green, blue, hue1, hue2, sat, lum;
float currenthue = 0.0;

int StrToHex(char str[])
{
	return (int)strtol(str, 0, 16);
}

void setup() {
	Serial.begin(BAUD_RATE);
}

void parseInstruction(String data) {
	if (data[0] == (char)0) {
		Serial.write("OK;");
	}
	else if (data[0] == (char)1) {
		Serial.write(ENDPOINT_NAME);
		Serial.write("&0;");
	}
	else if (data[0] == (char)2) {
		if (data[1] == 0) {
			char redStr[2], greenStr[2], blueStr[2];
			data.substring(2, 3).toCharArray(redStr, 2);
			data.substring(4, 5).toCharArray(greenStr, 2);
			data.substring(6, 7).toCharArray(blueStr, 2);
			setLedRgb(StrToHex(redStr), StrToHex(greenStr), StrToHex(blueStr));
			isGradient = false;
		}
		else if (data[1] == 1) {
			char hue1Str[2], hue2Str[2], satStr[2], lumStr[2];
			data.substring(2, 3).toCharArray(hue1Str, 2);
			data.substring(4, 5).toCharArray(hue2Str, 2);
			data.substring(6, 7).toCharArray(satStr, 2);
			data.substring(8, 9).toCharArray(lumStr, 2);
			setLedHsl(StrToHex(hue1Str), StrToHex(satStr), StrToHex(lumStr));
			isGradient = true;
		}
	}
}

void setLedRgb(int red, int green, int blue) {
	analogWrite(RED_PIN, red);
	analogWrite(GREEN_PIN, green);
	analogWrite(BLUE_PIN, blue);
}

void setLedHsl(byte h, byte s, byte v) {
	unsigned int i = h / 32;   // We want a value of 0 thru 5
	unsigned int f = (h % 32) * 8;   // 'fractional' part of 'i' 0..248 in jumps

	unsigned int sInv = 255 - s;  // 0 -> 0xff, 0xff -> 0
	unsigned int fInv = 255 - f;  // 0 -> 0xff, 0xff -> 0
	byte pv = v * sInv / 256;  // pv will be in range 0 - 255
	byte qv = v * (256 - s * f / 256) / 256;
	byte tv = v * (256 - s * fInv / 256) / 256;

	switch (i) {
	case 0:
		analogWrite(RED_PIN, v);
		analogWrite(GREEN_PIN, tv);
		analogWrite(BLUE_PIN, pv);
		break;
	case 1:
		analogWrite(RED_PIN, qv);
		analogWrite(GREEN_PIN, v);
		analogWrite(BLUE_PIN, pv);
		break;
	case 2:
		analogWrite(RED_PIN, pv);
		analogWrite(GREEN_PIN, v);
		analogWrite(BLUE_PIN, tv);
		break;
	case 3:
		analogWrite(RED_PIN, pv);
		analogWrite(GREEN_PIN, qv);
		analogWrite(BLUE_PIN, v);
		break;
	case 4:
		analogWrite(RED_PIN, tv);
		analogWrite(GREEN_PIN, pv);
		analogWrite(BLUE_PIN, v);
		break;
	case 5:
		analogWrite(RED_PIN, v);
		analogWrite(GREEN_PIN, pv);
		analogWrite(BLUE_PIN, qv);
		break;
	}
}

void loop() {
	while (Serial.available() > 0) {
		buffer += Serial.read();
		if (buffer.charAt(buffer.length() - 1) == ';') {
			char bufferChar[99];
			parseInstruction(buffer);
			buffer = "";
			break;
		}
	}

	if (isGradient) {
		if (gradientDirection) {
			if (hue1 > hue2) {
				currenthue -= 0.1;
			}
			else {
				currenthue += 0.1;
			}
		}
		currenthue += 0.1;
		setLedHsl(currenthue, sat, lum);	
	}
	delay(10);
}