package main

import (
	"fmt"
	"net"
	"os"
	"os/signal"
	"sync"
	"syscall"
)

func main() {
	host := "127.0.0.5"
	port := 8080

	address := fmt.Sprintf("%s:%d", host, port)

	listener, err := net.Listen("tcp", address)
	if err != nil {
		fmt.Printf("Failed to start the server: %s\n", err.Error())
		return
	}

	fmt.Printf("Server started on %s\n", address)

	var wg sync.WaitGroup
	wg.Add(1)

	go startClient(&wg, host, port)

	conn, err := listener.Accept()
	if err != nil {
		fmt.Printf("Error accepting connection: %s\n", err.Error())
		return
	}

	handleConnection(conn)

	wg.Wait()

	// Wait for termination signal before closing the TCP connection
	waitForTerminationSignal()

	conn.Close()
	fmt.Println("TCP connection closed")
}

func handleConnection(conn net.Conn) {
	defer conn.Close()

	buffer := make([]byte, 1024)
	_, err := conn.Read(buffer)
	if err != nil {
		fmt.Printf("Error reading data: %s\n", err.Error())
		return
	}

	data := string(buffer)
	fmt.Printf("Received data: %s\n", data)

	response := "Hello, client!"
	_, err = conn.Write([]byte(response))
	if err != nil {
		fmt.Printf("Error sending response: %s\n", err.Error())
		return
	}
}

func startClient(wg *sync.WaitGroup, host string, port int) {
	defer wg.Done()

	address := fmt.Sprintf("%s:%d", host, port)

	conn, err := net.Dial("tcp", address)
	if err != nil {
		fmt.Printf("Failed to connect to the server: %s\n", err.Error())
		return
	}

	defer conn.Close()

	message := "Hello, server!"
	_, err = conn.Write([]byte(message))
	if err != nil {
		fmt.Printf("Error sending data to the server: %s\n", err.Error())
		return
	}

	buffer := make([]byte, 1024)
	_, err = conn.Read(buffer)
	if err != nil {
		fmt.Printf("Error reading response from the server: %s\n", err.Error())
		return
	}

	response := string(buffer)
	fmt.Printf("Received response from server: %s\n", response)
}

func waitForTerminationSignal() {
	c := make(chan os.Signal, 1)
	signal.Notify(c, os.Interrupt, syscall.SIGTERM)
	<-c
}
