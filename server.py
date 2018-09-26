import socket
from socket import AF_INET, SOCK_DGRAM
import var
import checker

welcomeMessage = "Wellcome to the server"

def decode(data):
	return data.decode('utf-8').split("|")
	
def encode(data):
	return "|".join(data)

s = socket.socket(AF_INET, SOCK_DGRAM)
s.bind(("", var.port))
#s.listen(10)#10 koneksi yg diterima
s.settimeout(30)
timeoutCount = 0

clients = []
clientsAddr = []
clientsByAddress = {}

print("Starting the server...")
psn = ""
while psn != "exit":
	try:
		#client, addr = s.accept()
		#data = client.recv(var.buff)
		data, addr = s.recvfrom(var.buff)
		user, password, psn = decode(data)
		if psn == ">>>masuk<<<":
			if addr not in clientsAddr:
				client = socket.socket(AF_INET, SOCK_DGRAM)
				client.connect(addr)
				clients.append(client)
				clientsAddr.append(addr)
				clientsByAddress[addr] = client
				cdata = encode(["server", welcomeMessage])
				client.sendall(cdata.encode("utf-8"))
				
				print("apending new client")
		print("menerima pesan raw {0} dari {1}".format(str(data), str(addr)))
		timeoutCount = 0
		'''
		'''
		if psn != ">>>masuk<<<":
			if psn == ">>>RequestingTermination<<<":
				if addr in clientsByAddress:
					cdata = encode(["server", ">>>exit<<<"])
					clientsByAddress[addr].sendall(cdata.encode("utf-8"))
					clientsByAddress[addr].close()
					del clients[clients.index(clientsByAddress[addr])]
					del clientsAddr[clientsAddr.index(addr)]
					del clientsByAddress[addr]
				else:
					print("alamat {0} meminta keluar".format(str(addr)))
			else:
				for i in clients:
					if i.getpeername() != addr:
						ldata = decode(data)
						cdata = encode([ldata[0], ldata[2]])
						i.sendall(cdata.encode("utf-8"))
	except socket.timeout:
		print("server is timeout")
		timeoutCount += 1
		if timeoutCount > 5:
			for i in clients:
				cdata = encode(["server", "exit"])
				i.sendall(cdata.encode("utf-8"))
			s.close()
			break
	except:
		checker.getInfo()
		s.close()
		break