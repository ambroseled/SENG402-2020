#include <SPI.h>
#include <Ethernet.h>
#include <EthernetUdp.h>

// Enter a MAC address and IP address for your controller below.
// The IP address will be dependent on your local network:
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
//IPAddress ip(132, 181, 60, 166);
IPAddress ip(192, 168, 1, 1);
// IPAddress server();  // server IP address

unsigned int localPort = 8888;      // local port to listen on

// buffers for receiving and sending data
char packetBuffer[UDP_TX_PACKET_MAX_SIZE];  // buffer to hold incoming packet,
//char ReplyBuffer[] = "acknowledged";        // a string to send back

// An EthernetUDP instance to let us send and receive packets over UDP
EthernetUDP Udp;

#define NUM_SOLENOIDS  5
char solenoids[NUM_SOLENOIDS];

int solenoidPin1=19;
int solenoidPin2=20;
int solenoidPin3=21;
int solenoidPin4=22;
int solenoidPin5=23;

void setup() {
  // put your setup code here, to run once:
  pinMode(solenoidPin1,OUTPUT);
  pinMode(solenoidPin2,OUTPUT);
  pinMode(solenoidPin3,OUTPUT);
  pinMode(solenoidPin4,OUTPUT);
  pinMode(solenoidPin5,OUTPUT);

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
//    Serial.print("Received packet of size ");
//    Serial.println(packetSize);
//    Serial.print("From ");
//    IPAddress remote = Udp.remoteIP();
//    for (int i=0; i < 4; i++) {
//      Serial.print(remote[i], DEC);
//      if (i < 3) {
//        Serial.print(".");
//      }
//    }
//    Serial.print(", port ");
//    Serial.println(Udp.remotePort());

    // read the packet into packetBuffer
    Udp.read(packetBuffer, UDP_TX_PACKET_MAX_SIZE);
//    Serial.println("Contents:");
//    Serial.println(packetBuffer);

    // send a reply to the IP address and port that sent us the packet we received
    //Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());
    //Udp.write(ReplyBuffer);
    //Udp.endPacket();

    // update the solenoid array from the received packet
    for (int i=0; i<=4; i+=1) {
      solenoids[i] = packetBuffer[i];
    }
  }
  
  for (int i=0; i<=4; i+=1) {
    if (solenoids[i] == '1') {
      switch_nozzle(i);
    } else {
      shutoff_nozzle(i);
    }
  }  
}

void switch_nozzle(int i)
{
  if (i == 0) {
    digitalWrite(solenoidPin1, HIGH);
  } else if (i == 1) {
    digitalWrite(solenoidPin2, HIGH);
  } else if (i == 2) {
    digitalWrite(solenoidPin3, HIGH);
  } else if (i == 3) {
    digitalWrite(solenoidPin4, HIGH);
  } else if (i == 4) {
    digitalWrite(solenoidPin5, HIGH);
  }
}

void shutoff_nozzle(int i)
{
  if (i == 0) {
    digitalWrite(solenoidPin1, LOW);
  } else if (i == 1) {
    digitalWrite(solenoidPin2, LOW);
  } else if (i == 2) {
    digitalWrite(solenoidPin3, LOW);
  } else if (i == 3) {
    digitalWrite(solenoidPin4, LOW);
  } else if (i == 4) {
    digitalWrite(solenoidPin5, LOW);
  }
}
