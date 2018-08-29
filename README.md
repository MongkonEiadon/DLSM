# แจ้งปิด (down) เครื่อง Test Server 192.168.252.109 เพื่อ BackUp ไฟล์ VM
เพิ่มเติม [คลิ๊ก](https://gitlab.com/nysiis-solutions/driver-license-stock-management/issues/14)

### Production Server Specification
Windows Server 2016  
Web Server IIS version 10  
.Net 4.5  
MSSQL 2017

### ไฟล์ฐานข้อมูล
- เป็นไฟล์ .bak อยู่ที่ [root] ของ project ชื่อว่าไฟล์ [:paperclip: bak_DLSM20180824.bak](https://gitlab.com/nysiis-solutions/driver-license-stock-management/blob/master/bak_DLSM20180824.bak)

### การส่งงาน
- หลังจากทำงานที่ localhost เรียบร้อยแล้ว ให้ push งานเข้ามาที่ GitLab แล้ว build โปรเจ็คไปวางที่ Test Server เพื่อให้เจ้าหน้าที่ตรวจรับ)
- หากมีการ update store หรือ sql ให้วางใว้ที่โฟลเดอร์ `driver-license-stock-management > DLSM > DLSM > db_patch`

:paperclip: [HowToUsePptpVpn.docx](https://adiwitcoth.sharepoint.com/:w:/g/ESrmHgXT6qJFtHASaLAVE8kB3UjpN95gduKC0P5eYPt6HA?e=Ug8SOI)

ต่อ vpn ไปที่ `118.175.32.248`  
user : `uservpn`  
password : `12345678`  

แล้ว RDP ไปที่ `192.168.252.109`  
user : `administrator`  
password : `P@ssw0rd`