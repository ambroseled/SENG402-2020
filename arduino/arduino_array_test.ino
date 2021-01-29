#include <FastLED.h>
#include <SPI.h>
#include <Ethernet.h>

#define L_LED_PIN     7
#define R_LED_PIN     8
#define NUM_LEDS    16
CRGB leftleds[NUM_LEDS];
CRGB rightleds[NUM_LEDS];

EthernetClient client;
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
byte ip[] = { 132, 181, 60, 147 };
byte server[] = { 64, 233, 187, 99 }; // Google

bool solenoids[NUM_LEDS+1];
int solenoidPin=23;

void setup() {
  // put your setup code here, to run once:
  pinMode(solenoidPin,OUTPUT);
  FastLED.addLeds<WS2812, L_LED_PIN, GRB>(leftleds, NUM_LEDS);
  FastLED.addLeds<WS2812, R_LED_PIN, GRB>(rightleds, NUM_LEDS);

  // Startup LED check
  for (int i = 14; i >= 0; i-=2) {
    leftleds[i] = CRGB(0, 0, 255);
    rightleds[i] = CRGB(0, 0, 255);
    FastLED.show();
    delay(300);
  }
  //FastLED.clear();
  for (int i = 14; i >= 0; i-=2) {
    leftleds[i] = CRGB(0, 255, 0);
    rightleds[i] = CRGB(0, 255, 0);
    FastLED.show();
  }
  delay(1000);
  
  // Turn off ready for sim
  FastLED.clear();
  FastLED.show();

  Ethernet.begin(mac, ip);

  // Check for Ethernet hardware present
  if (Ethernet.hardwareStatus() == EthernetNoHardware) 
  {
    Serial.println("Ethernet shield was not found.  Sorry, can't run without hardware.");
    while (true) 
    {
      delay(1); // do nothing, no point running without Ethernet hardware
    }
  }

  if (Ethernet.linkStatus() == LinkOFF) 
  {
    Serial.println("Ethernet cable is not connected.");
  }

  Serial.begin(9600);
  
  Serial.println("\nConnecting...");

  if (client.connect(server, 80)) {
    Serial.println("connected");
    client.println("Hello");
  } else {
    Serial.println("Error: Connection failed");
  }
}

void loop() {
  // put your main code here, to run repeatedly:

    if (client.available()) {
      char c = client.read();
      Serial.print(c);
    }

    if (!client.connected()) {
      Serial.println();
      Serial.println("No Ethernet Connection!\nDisconnecting and starting demo.");
      client.stop();
      for(;;)
        demo();
    }
}
  
void demo() {
  // Demo loop - randomly turns LED banks on
  array_on(); // Picking LEDs in array to turn on
  
  int j = 0;
  for (int i=0; i<=16; i+=1) {
    if (i == 8) {
      if (solenoids[i] == 1) {
        switch_nozzle();
      } else {
        shutoff_nozzle();
      }
    } else {
      if (solenoids[i] == 1) {
        switch_led(j);
      } else {
        shutoff_led(j);
      }
      j += 2;
    }
  }

  array_off();

  int k = 0;
  for (int i=0; i<=16; i+=1) {
    if (i == 8) {
      if (solenoids[i] == 1) {
        switch_nozzle();
      } else {
        shutoff_nozzle();
      }
    } else {
      if (solenoids[i] == 1) {
        switch_led(k);
      } else {
        shutoff_led(k);
      }
      k += 2;
    }
  }
  
}

void switch_nozzle()
{
  digitalWrite(solenoidPin, HIGH);
}

void shutoff_nozzle()
{
  digitalWrite(solenoidPin, LOW);
}

void switch_led(int i)
{
  if (i < 16) {
    leftleds[i] = CRGB(0, 255, 0);
    FastLED.show();
  }
  else { 
    int led_index = 30 - (i);
    rightleds[led_index] = CRGB(0, 255, 0);
    FastLED.show();
  }
}

void shutoff_led(int i)
{
  // Turning off indexed LEDs
  if (i < 16) {
    // Left bank
    leftleds[i] = CRGB(0, 0, 0);
    FastLED.show();
  }
  else { 
    // Right bank
    int led_index = 30 - (i);
    rightleds[led_index] = CRGB(0, 0, 0);
    FastLED.show();
  }
}

void array_on() {
  // Setting random group of solenoids to on
  delay(1000);
  
  int i = random(0,17); // Random point on boom
  int w = random(1,3);  // Grouping 3 or 5 nozzles together
  int upper = i+w;  // Upper bank bound
  int lower = i-w;  // Lower bank bound

  // Limiting to edges of the boom
  if (upper>16) {
    upper = 16;
  } else if (lower<0){
    lower = 0; 
  }

  // Setting all nozzles between bounds to on
  for (i=lower; i<=upper; i+=1) {
    solenoids[i] = 1;
  }
}

void array_off() {
  // Setting entire array to zero
  delay(1000);
  memset(solenoids,0,sizeof(solenoids));
}
