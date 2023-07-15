package main

import (
	"flag"
	"fmt"
	"io"
	"log"
	"net/url"
	"os"
	"time"

	"github.com/gorilla/websocket"
)

var (
	ip          = flag.String("ip", "127.0.0.1", "server IP")
	connections = flag.Int("conn", 1, "number of websocket connections")
)

func main() {
	flag.Usage = func() {
		io.WriteString(os.Stderr, `Websockets client generator
Example usage: ./client -ip=172.17.0.1 -conn=10
`)
		flag.PrintDefaults()
	}
	flag.Parse()

	u := url.URL{Scheme: "ws", Host: "127.0.0.1/" + *ip, Path: "/"}
	log.Printf("Connecting to %s", u.String())

	tts := time.Second * 5

	for i := 0; i < *connections; i++ {
		go createWebSocketConnection(u.String(), i, tts)
	}

	select {}
}

func createWebSocketConnection(url string, connIndex int, tts time.Duration) {
	c, _, err := websocket.DefaultDialer.Dial(url, nil)
	if err != nil {
		fmt.Printf("Failed to connect %d: %v\n", connIndex, err)
		return
	}
	defer func() {
		c.WriteControl(websocket.CloseMessage, websocket.FormatCloseMessage(websocket.CloseNormalClosure, ""), time.Now().Add(time.Second))
		time.Sleep(time.Second)
		c.Close()
	}()

	log.Printf("Conn %d established", connIndex)

	for {
		time.Sleep(tts)
		log.Printf("Conn %d sending message", connIndex)

		if err := c.WriteControl(websocket.PingMessage, nil, time.Now().Add(time.Second*5)); err != nil {
			fmt.Printf("Failed to receive pong for Conn %d: %v\n", connIndex, err)
			break
		}

		err := c.WriteMessage(websocket.TextMessage, []byte(fmt.Sprintf("Hello %v", connIndex)))
		if err != nil {
			fmt.Printf("Failed to send message from Conn %d: %v\n", connIndex, err)
			break
		}
	}

	log.Printf("Conn %d closed", connIndex)
}
