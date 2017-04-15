#include <ESP8266WiFi.h>
#include <WiFiClient.h> 
#include <ESP8266WebServer.h>
#include <WebSocketsServer.h>

const char *ssid = "ESP8266";
const char *password = "123456789";

ESP8266WebServer server(80);
WebSocketsServer socketserver(69);
IPAddress localIP = IPAddress(192, 168, 1, 69);
IPAddress gatewayIP = IPAddress(192, 168, 1, 1);
IPAddress subnetIP = IPAddress(255, 255, 255, 0);
//IPAddress 

void handleRoot() {
	server.send(200, "text/html", "<h1>You are connected</h1>");
}

void webSocketEvent(uint8_t num, WStype_t type, uint8_t * payload, size_t length) {

	switch (type) {
	case WStype_DISCONNECTED:
		Serial.printf("[%u] Disconnected!\n", num);
		break;
	case WStype_CONNECTED: {
		IPAddress ip = socketserver.remoteIP(num);
		Serial.printf("[%u] Connected from %d.%d.%d.%d url: %s\n", num, ip[0], ip[1], ip[2], ip[3], payload);

		// send message to client
		socketserver.sendTXT(num, "Connected");
	}
						   break;
	case WStype_TEXT:
		Serial.printf("[%u] get Text: %s\n", num, payload);

		if (payload[0] == '#') {
		}

		break;
	}

}

void setup() {
	delay(1000);
	Serial.begin(115200);
	Serial.println();

	WiFi.softAP(ssid, password);
	WiFi.softAPConfig(localIP, gatewayIP, subnetIP);
	WiFi.config(localIP, gatewayIP, subnetIP);
	server.on("/",handleRoot);
	server.begin();
	socketserver.onEvent(webSocketEvent);
	socketserver.begin();
}

void loop() {
	server.handleClient();
}
