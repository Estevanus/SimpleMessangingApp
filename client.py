import socket
from socket import AF_INET, SOCK_DGRAM
import var

s = socket.socket(AF_INET, SOCK_DGRAM)

def decode(data):
	return data.decode('utf-8').split("|")
	
def encode(data):
	return "|".join(data)

username = "no nama"
password = ""

def encodeAndSend(data):
	s.send(encode([username, password, data]).encode('utf-8'))

print("Selamat datang di messanging sederhana ini")
print("Sebelumnya silakan masukan ip servernya")
ip = input("IP >>")
print("Sekarang masukan username dan passwordmu")
username = input("username :")
password = input("password :")

laddr = None
s.connect((ip, var.port))
laddr = s.getsockname()
listener = socket.socket(AF_INET, SOCK_DGRAM)
listener.bind(laddr)
s.settimeout(30)
encodeAndSend(">>>masuk<<<")
print("login done, your client info is " + str(s))
print("getsockname ialah " + str(s.getsockname()))


psn = ""
while psn != "exit":
	psn = input(username + " >>")
	if psn == "cek pesan":
		data = listener.recv(var.buff)
		print(data)
		h = decode(data)
		if h[0] == "server" and h[1] == "exit":
			s.close()
			psn = "exit"
			break
	else:
		encodeAndSend(psn)
	
encodeAndSend("exit")