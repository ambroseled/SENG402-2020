#include <FastLED.h>
#include <SPI.h>
#include <Ethernet.h>
#include <EthernetUdp.h>

// Enter a MAC address and IP address for your controller below.
// The IP address will be dependent on your local network:
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
IPAddress ip(192, 168, 1, 24);
// IPAddress server();  // server IP address

unsigned int localPort = 8888;      // local port to listen on

// buffers for receiving and sending data
char packetBuffer[UDP_TX_PACKET_MAX_SIZE];  // buffer to hold incoming packet,
//char ReplyBuffer[] = "acknowledged";        // a string to send back

// An EthernetUDP instance to let us send and receive packets over UDP
EthernetUDP Udp;

#define L_LED_PIN     7
#define R_LED_PIN     8
#define NUM_LEDS    16
CRGB leftleds[NUM_LEDS];
CRGB rightleds[NUM_LEDS];

char solenoids[NUM_LEDS+1];
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
  delay(1000);

  // Turn off ready for sim
  FastLED.clear();
  FastLED.show();
  delay(500);

  // start the Ethernet
  Ethernet.begin(mac, ip);

  // Open serial communications and wait for port to open:
  Serial.begin(9600);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }

  // Check for Ethernet hardware present
  if (Ethernet.hardwareStatus() == EthernetNoHardware) {
    Serial.println("Ethernet shield was not found.  Sorry, can't run without hardware. :(");
    while (true) {
      delay(1); // do nothing, no point running without Ethernet hardware
    }
  }
  if (Ethernet.linkStatus() == LinkOFF) {
    Serial.println("Ethernet cable is not connected.");
  }

  // start UDP
  Udp.begin(localPort);
}

void loop() {
  // put your main code here, to run repeatedly:

  // if there's data available, read a packet
  int packetSize = Udp.parsePacket();
  if (packetSize) {
    Serial.print("Received packet of size ");
    Serial.println(packetSize);
    Serial.print("From ");
    IPAddress remote = Udp.remoteIP();
    for (int i=0; i < 4; i++) {
      Serial.print(remote[i], DEC);
      if (i < 3) {
        Serial.print(".");
      }
    }
    Serial.print(", port ");
    Serial.println(Udp.remotePort());

    // read the packet into packetBuffer
    Udp.read(packetBuffer, UDP_TX_PACKET_MAX_SIZE);
    Serial.println("Contents:");
    Serial.println(packetBuffer);

    // send a reply to the IP address and port that sent us the packet we received
    //Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());
    //Udp.write(ReplyBuffer);
    //Udp.endPacket();

    // update the solenoid array from the received packet
    for (int i=0; i<=16; i+=1) {
      solenoids[i] = packetBuffer[i];
    }
  }
  
  int j = 0;
  for (int i=0; i<=16; i+=1) {
    if (i == 8) {
      if (solenoids[i] == '1') {
        switch_nozzle();
      } else {
        shutoff_nozzle();
      }
    } else {
      if (solenoids[i] == '1') {
        switch_led(j);
      } else {
        shutoff_led(j);
      }
      j += 2;
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
  if (i < 16) {
    leftleds[i] = CRGB(0, 0, 0);
    FastLED.show();
  }
  else { 
    int led_index = 30 - (i);
    rightleds[led_index] = CRGB(0, 0, 0);
    FastLED.show();
  }
}
