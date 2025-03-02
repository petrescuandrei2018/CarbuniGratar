# CarbuniGratar
Refacerea site-ului https://carbunipentrugratar.ro/ în C# .NET 8 cu ASP.NET Core MVC + Razor Pages (Mobile-First)

📌 **Obiectiv general:**
- ✅ **90% din utilizatori sunt pe mobil** → Prioritizăm viteză și experiență fluidă.
- ✅ **SEO avansat** → Optimizare maximă pentru primele poziții în Google.
- ✅ **Proces de cumpărare ultra-rapid** → Checkout fără logare, dar beneficii pentru userii logați.
- ✅ **Sistem de review-uri complet** → Stele, text, poze, video, optimizat pentru încărcare rapidă.
- ✅ **Imagini și video de calitate mare fără a afecta performanța** → WebP + Lazy Loading + CDN.

---

## 🔹 **Plan de implementare pas cu pas (Mobile-First)**  

### 🟢 **Faza 1: Coș de cumpărături rapid & fără fricțiuni**  
📌 **Obiectiv:** Navigare și adăugare în coș ultra-rapidă, fără logare obligatorie.  

✅ **Local Storage pentru utilizatorii neautentificați** – Fără request-uri inutile la server.  
✅ **Sincronizare automată cu SQL Server pentru utilizatorii logați** – Coș salvat pe toate dispozitivele.  
✅ **Adăugare instantanee prin AJAX, fără refresh de pagină.**  
✅ **Afișare coș dinamic ca buton flotant, mereu accesibil.**  
✅ **Swipe pentru ștergere produse din coș (UX optimizat pentru mobile).**  

📌 **Tehnologii folosite:**  
✔️ `JavaScript (AJAX)` – Interacțiuni rapide.  
✔️ `Local Storage` – Coș persistent pe mobil.  
✔️ `SQL Server` – Sincronizare pentru userii logați.  

---

### 🟢 **Faza 2: Checkout ultra-rapid (Experiență mobile-friendly)**  
📌 **Obiectiv:** Finalizare comandă în maximum **30 de secunde**, fără obstacole.  

✅ **Formular minimizat cu doar 3 câmpuri:**  
- 📌 Nume  
- 📌 Telefon  
- 📌 Adresă livrare *(Autocomplete Google Places API)*  

✅ **Google Places API pentru autocompletare adresă** → Fără scriere manuală, doar selectare rapidă.  
✅ **Oferte pentru utilizatorii logați (ex: 5% reducere pentru cont).**  
✅ **Sincronizare rapidă a comenzii cu firmele de curierat (ex: FanCourier API).**  

📌 **Tehnologii folosite:**  
✔️ `Google Places API` – Autocomplete adresă.  
✔️ `AJAX` – Verificare rapidă a disponibilității stocului.  
✔️ `SQL Server` – Salvare rapidă a comenzilor.  

---

### 🟢 **Faza 3: Sistem de review-uri optimizat pentru mobil**  
📌 **Obiectiv:** Review-uri rapide, vizibile și ușor de postat pe mobil.  

✅ **Evaluare 5 stele + text review + upload rapid poze/video.**  
✅ **Integrare cu Cloud Storage (CDN) pentru stocare ultra-rapidă.**  
✅ **Afișare review-uri în schema Google pentru SEO (JSON-LD).**  
✅ **Interfață optimizată pentru mobile → Lazy Loading pentru poze/video.**  

📌 **Tehnologii folosite:**  
✔️ `CDN (Cloud Storage)` – Încărcare rapidă a media.  
✔️ `Lazy Loading` – Încărcare doar când utilizatorul o vede.  
✔️ `Schema Markup (JSON-LD)` – Review-urile apar direct în Google.  

---

### 🟢 **Faza 4: Optimizări pentru viteză & performanță**  
📌 **Obiectiv:** Site-ul trebuie să se încarce **în mai puțin de 1 secundă** pe mobil.  

✅ **Lazy Loading pentru imagini & videoclipuri** – Încărcare doar la scroll.  
✅ **Conversie automată WebP pentru imagini mari** – Reducere dimensiuni cu 50%.  
✅ **Caching agresiv cu Redis + GZIP/Brotli** – Viteză maximă.  
✅ **Preloading pentru resurse importante** (fonturi, CSS critic).  

📌 **Tehnologii folosite:**  
✔️ `WebP` – Imagini ultra-rapide.  
✔️ `Redis Cache` – Acces rapid la date.  
✔️ `Lazy Loading` – Încărcare media doar când utilizatorul o vede.  

---

### 🟢 **Faza 5: SEO puternic pentru mobile & trafic organic ridicat**  
📌 **Obiectiv:** Ajungem rapid pe **primele poziții în Google** pentru mobile.  

✅ **Pagină dedicată pentru fiecare produs, optimizată SEO.**  
✅ **Sitemap XML + Robots.txt optimizat pentru indexare rapidă.**  
✅ **Schema Markup pentru produse & review-uri (JSON-LD).**  
✅ **Creăm un blog cu articole relevante despre produse și utilizare.**  
✅ **Evităm pop-up-uri agresive** *(Google penalizează site-urile cu pop-up-uri invazive pe mobil).*  

---

### 🟢 **Faza 6: Fidelizare clienți & Creșterea conversiilor**  
📌 **Obiectiv:** Păstrăm clienții și îi încurajăm să revină.  

✅ **🔔 Notificări Push pentru promoții (mobile-friendly).**  
✅ **🎟️ Sistem de loialitate (Puncte pentru fiecare comandă).**  
✅ **📩 Email Marketing Automatizat (Recuperare coș abandonat).**  
✅ **🤝 Program de recomandare (Referral Program).**  

📌 **Tehnologii folosite:**  
✔️ `Firebase Notifications` – Notificări push.  
✔️ `Email Marketing API (Mailchimp, SendGrid)` – Emailuri automate.  

---

📌 **📢 Proiectul este în dezvoltare activă!**  
💬 **Contribuții, feedback și îmbunătățiri sunt binevenite!** 🚀  
