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

## เงื่อนไข และ ข้อกำหนด
- โปรโมชั่น: *ปิดงานได้ก่อนกำหนดเวลารับ bonus เพิ่ม +2% ค่าส่งงานเร็ว*
- รบกวน push งานครั้งแรกภายใน 6 ชม. หลังจากได้รับเมลอนุมัติเริ่มงาน
- หลังจากนั้นรบกวน push งานบ่อยๆ (ประมาณ 1-2ชม. ครั้ง หรืออาจน้อยกว่านั้นก็ได้ตามที่เห็นสมควร)
- ส่งงานช้ากว่ากำหนด ตั้งแต่ 00:01 - 24:00 ชม. หักค่าส่งงานช้า 50 บาท
- ส่งงานช้ากว่ากำหนดตั้งแต่ 24:01 ~ 48:00 ชม. หักค่าส่งงานช้าเพิ่ม 100 บาท + ดีเลย์การจ่ายเงินภายใน 3 วันฯเป็น 5 วันทำการ
- ส่งงานช้ากว่ากำหนดตั้งแต่ 48:01 ~ 72:00 ชม. หักค่าส่งงานช้าเพิ่ม 150 บาท + ดีเลย์การจ่ายเงินภายใน 3 วันฯเป็น 15 วันทำการ
- อนุญาตยกเลิกงานหากส่งงานช้ากว่ากำหนด 48:01 ชม. ขึ้นไป