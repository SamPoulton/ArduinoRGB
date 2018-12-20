#include <String.h>
#include <EEPROM.h>
const int BAUD_RATE = 9600;
const char DEVICE_NAME[] = "Test Device";

// EEPROM addresses
class memoryOffsets {
public:
	const static int MODE_LOC = 0;
	const static int RED_LOC = 1;
	const static int GREEN_LOC = 2;
	const static int BLUE_LOC = 3;
	const static int HUE1_LOC = 4;
	const static int HUE2_LOC = 5;
	const static int SAT_LOC = 6;
	const static int VAL_LOC = 7;
	const static int SPEED_LOC = 8;
};

class rgbEndpoint {
public:
	int redPin, greenPin, bluePin, eepromLoc, currenthue;
	unsigned long lastTime = 0;
	String endpointName;
	bool gradientDirection;
	rgbEndpoint(int redpin, int greenpin, int bluepin, char endpointname[], int memoryOffset) {
		redPin = redpin;
		greenPin = greenpin;
		bluePin = bluepin;
		endpointName = endpointname;
		eepromLoc = memoryOffset;
		if (EEPROM.read(eepromLoc + memoryOffsets::MODE_LOC) == 0) {
			analogWrite(redPin, EEPROM.read(eepromLoc + memoryOffsets::RED_LOC));
			analogWrite(greenPin, EEPROM.read(eepromLoc + memoryOffsets::GREEN_LOC));
			analogWrite(bluePin, EEPROM.read(eepromLoc + memoryOffsets::BLUE_LOC));
		}
	}
	void tick() {
		if (millis() - lastTime >= EEPROM.read(eepromLoc + memoryOffsets::SPEED_LOC)) {
			if (EEPROM.read(eepromLoc + memoryOffsets::MODE_LOC) == 1) {
				if (gradientDirection) {
					if (currenthue < EEPROM.read(eepromLoc + memoryOffsets::HUE1_LOC)) gradientDirection = false;
					else currenthue -= 1;
				}
				else {
					if (currenthue > EEPROM.read(eepromLoc + memoryOffsets::HUE2_LOC)) gradientDirection = true;
					else currenthue += 1;
				}
				setLedHsl(currenthue, EEPROM.read(eepromLoc + memoryOffsets::SAT_LOC), EEPROM.read(eepromLoc + memoryOffsets::VAL_LOC));
			}
			lastTime = millis();
		}
	}
	void setLedRgb(byte red, byte green, byte blue) {
		EEPROM.write(eepromLoc + memoryOffsets::MODE_LOC, 0);
		EEPROM.write(eepromLoc + memoryOffsets::RED_LOC, red);
		EEPROM.write(eepromLoc + memoryOffsets::GREEN_LOC, green);
		EEPROM.write(eepromLoc + memoryOffsets::BLUE_LOC, blue);
		analogWrite(redPin, red);
		analogWrite(greenPin, green);
		analogWrite(bluePin, blue);
	}

	void setGradient(byte hue1, byte hue2, byte saturation, byte value, byte fadespeed) {
		EEPROM.write(eepromLoc + memoryOffsets::MODE_LOC, 1);
		EEPROM.write(eepromLoc + memoryOffsets::HUE1_LOC, hue1);
		EEPROM.write(eepromLoc + memoryOffsets::HUE2_LOC, hue2);
		EEPROM.write(eepromLoc + memoryOffsets::SAT_LOC, saturation);
		EEPROM.write(eepromLoc + memoryOffsets::VAL_LOC, value);
		EEPROM.write(eepromLoc + memoryOffsets::SPEED_LOC, 255 - fadespeed);
		setLedHsl(hue1, saturation, value);
    currenthue = hue1;
	}

	void setLedHsl(byte h, byte s, byte v) {
		h = (h * 192) / 256;  // 0..191
		unsigned int i = h / 32;   // We want a value of 0 thru 5
		unsigned int f = (h % 32) * 8;   // 'fractional' part of 'i' 0..248 in jumps

		unsigned int sInv = 255 - s;  // 0 -> 0xff, 0xff -> 0
		unsigned int fInv = 255 - f;  // 0 -> 0xff, 0xff -> 0
		byte pv = v * sInv / 256;  // pv will be in range 0 - 255
		byte qv = v * (256 - s * f / 256) / 256;
		byte tv = v * (256 - s * fInv / 256) / 256;

		switch (i) {
		case 0:
			analogWrite(redPin, v);
			analogWrite(greenPin, tv);
			analogWrite(bluePin, pv);
			break;
		case 1:
			analogWrite(redPin, qv);
			analogWrite(greenPin, v);
			analogWrite(bluePin, pv);
			break;
		case 2:
			analogWrite(redPin, pv);
			analogWrite(greenPin, v);
			analogWrite(bluePin, tv);
			break;
		case 3:
			analogWrite(redPin, pv);
			analogWrite(greenPin, qv);
			analogWrite(bluePin, v);
			break;
		case 4:
			analogWrite(redPin, tv);
			analogWrite(greenPin, pv);
			analogWrite(bluePin, v);
			break;
		case 5:
			analogWrite(redPin, v);
			analogWrite(greenPin, pv);
			analogWrite(bluePin, qv);
			break;
		}

	}
};

rgbEndpoint endpoints[2] = {
rgbEndpoint(5, 6, 3, "Pins 5-6-3", 0),
// 5, 6, 3
rgbEndpoint(11, 9, 10, "Pins 11-9-10", 9) };
// 10, 11, 9
String dataBuffer = "";


int StrToHex(char str[])
{
	return (int)strtol(str, 0, 16);
}
int StrToInt(char str[]) {
	return (int)strtol(str, 0, 10);
}
void parseInstruction(String data) {
	// Serial.print("Instruction type is ");
    // Serial.println(data[0]);
	if (data[0] == '0') {
		if (data[1] == '0') {
			Serial.print("OK;");
		}
		else if (data[1] == '1') {
			Serial.print(DEVICE_NAME);
			Serial.print(';');
		}
	}
	else if (data[0] == '1') {
		for (int i = 0; i <= 1; i++) {
			if (i > 0) Serial.print(",");
			Serial.print(endpoints[i].endpointName.c_str());
			Serial.print("&");
			Serial.print(i, DEC);
			Serial.print("&");
			if (EEPROM.read(endpoints[i].eepromLoc + memoryOffsets::MODE_LOC) == 1) {
				Serial.print(1, DEC);
				Serial.print("&");
				Serial.print(EEPROM.read(endpoints[i].eepromLoc + memoryOffsets::HUE1_LOC), HEX);
				Serial.print("&");
				Serial.print(EEPROM.read(endpoints[i].eepromLoc + memoryOffsets::HUE2_LOC), HEX);
				Serial.print("&");
				Serial.print(EEPROM.read(endpoints[i].eepromLoc + memoryOffsets::SAT_LOC), HEX);
				Serial.print("&");
				Serial.print(EEPROM.read(endpoints[i].eepromLoc + memoryOffsets::VAL_LOC), HEX);
				Serial.print("&");
				Serial.print(255 - EEPROM.read(endpoints[i].eepromLoc + memoryOffsets::SPEED_LOC), HEX);
			}
			else {
				Serial.print(0, DEC);
				Serial.print("&");
				Serial.print(EEPROM.read(endpoints[i].eepromLoc + memoryOffsets::RED_LOC), HEX);
				Serial.print("&");
				Serial.print(EEPROM.read(endpoints[i].eepromLoc + memoryOffsets::GREEN_LOC), HEX);
				Serial.print("&");
				Serial.print(EEPROM.read(endpoints[i].eepromLoc + memoryOffsets::BLUE_LOC), HEX);
			}
		}
		Serial.print(";");
	}
	else if (data[0] == '2') {
		int endpointIndex = data[2] - '0';
		if (data[1] == '0') {
			EEPROM.write(memoryOffsets::MODE_LOC, 0);
			char redStr[3], greenStr[3], blueStr[3];
			data.substring(3, 5).toCharArray(redStr, 3);
			data.substring(5, 7).toCharArray(greenStr, 3);
			data.substring(7, 9).toCharArray(blueStr, 3);

			int redValue = StrToHex(redStr);
			int greenValue = StrToHex(greenStr);
			int blueValue = StrToHex(blueStr);

			endpoints[endpointIndex].setLedRgb(redValue, greenValue, blueValue);
			Serial.print("OK;");
		}
		else if (data[1] == '1') {
			char hue1Str[3], hue2Str[3], satStr[3], valStr[3], speedStr[3];
			data.substring(3, 5).toCharArray(hue1Str, 3);
			data.substring(5, 7).toCharArray(hue2Str, 3);
			data.substring(7, 9).toCharArray(satStr, 3);
			data.substring(9, 11).toCharArray(valStr, 3);
			data.substring(11, 13).toCharArray(speedStr, 3);

			int hue1Value = StrToHex(hue1Str);
			int hue2Value = StrToHex(hue2Str);
			int satValue = StrToHex(satStr);
			int valValue = StrToHex(valStr);
			int speedValue = StrToHex(speedStr);
      if (speedValue ==0) speedValue++;
			endpoints[endpointIndex].setGradient(hue1Value, hue2Value, satValue, valValue, speedValue);
			Serial.print("OK;");
		}
	}
}

void setup() {
	Serial.begin(BAUD_RATE);
	Serial.setTimeout(100);
}
void loop() {
	if (Serial.available()) {
		dataBuffer += Serial.readString();
		if (dataBuffer.charAt(dataBuffer.length() - 1) == ';') {
			parseInstruction(dataBuffer);
			dataBuffer = "";
		}
	}
	for (int i = 0; i <= 1; i++) {
		endpoints[i].tick();
	}
}
