#include <String.h>
const int BAUD_RATE = 9600;
const char DEVICE_NAME[] = "Test Device";
const char ENDPOINT_NAME[] = "Test Header";

const int RED_PIN = 9;
const int GREEN_PIN = 10;
const int BLUE_PIN = 11;

//debug leds
class debugLed {
  private:
  int pin;
  bool isOn;

  public:
  void toggle() {
    if (isOn) {
      digitalWrite(pin, LOW);
      isOn = false;
    } else {
      digitalWrite(pin, HIGH);
      isOn = true;
    }
  }
  debugLed(int pin) {
	  this->pin = pin;
	  this->isOn = false;
  }
};

debugLed redLed(2);
debugLed yellowLed(3);
debugLed greenLed(4);
debugLed blueLed(5);

String dataBuffer = "";

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
  Serial.setTimeout(100);
}

void parseInstruction(String data) {
 // Serial.print("Instruction type is ");
//  Serial.println(data[0]);
	if (data[0] == '0') {
		Serial.print("OK;");
		redLed.toggle();
	}
	else if (data[0] == '1') {
		Serial.print(ENDPOINT_NAME);
		Serial.print("&0;");
		yellowLed.toggle();
	}
	else if (data[0] == '2') {
		if (data[1] == '0') {
			char redStr[3], greenStr[3], blueStr[3];
			data.substring(3, 5).toCharArray(redStr, 3);
			data.substring(5, 7).toCharArray(greenStr, 3);
			data.substring(7, 9).toCharArray(blueStr, 3);

      int redValue = StrToHex(redStr);
      int greenValue = StrToHex(greenStr);
      int blueValue = StrToHex(blueStr);
      
			setLedRgb(redValue, greenValue, blueValue);
			isGradient = false;
     /*
      Serial.print("Red: 0x");
      Serial.print(redStr);
      Serial.print(", equiv to ");
      Serial.println(redValue);
      
      Serial.print("Green: 0x");
      Serial.print(greenStr);
      Serial.print(", equiv to ");
      Serial.println(greenValue);

      Serial.print("Blue: 0x");
      Serial.print(blueStr);
      Serial.print(", equiv to ");
      Serial.println(blueValue);
      */
      Serial.print("OK;");
		}
		else if (data[1] == '1') {
			char hue1Str[2], hue2Str[2], satStr[2], lumStr[2];
			data.substring(2, 3).toCharArray(hue1Str, 2);
			data.substring(4, 5).toCharArray(hue2Str, 2);
			data.substring(6, 7).toCharArray(satStr, 2);
			data.substring(8, 9).toCharArray(lumStr, 2);
			setLedHsl(StrToHex(hue1Str), StrToHex(satStr), StrToHex(lumStr));
			isGradient = true;
		}
		greenLed.toggle();
	}
}

void setLedRgb(int inred, int ingreen, int inblue) {
  red = inred;
  green = ingreen;
  blue = inblue;
	analogWrite(RED_PIN, inred);
	analogWrite(GREEN_PIN, ingreen);
	analogWrite(BLUE_PIN, inblue);

  /* Serial.print("Written tuple ");
  Serial.print(inred);
  Serial.print(", ");
  Serial.print(ingreen);
  Serial.print(", ");
  Serial.println(inblue); */
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
	if (Serial.available()) {
    blueLed.toggle();
		dataBuffer += Serial.readString();
		if (dataBuffer.charAt(dataBuffer.length() - 1) == ';') {
			parseInstruction(dataBuffer);
			dataBuffer = "";
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
  blueLed.toggle();
	delay(100);
}
