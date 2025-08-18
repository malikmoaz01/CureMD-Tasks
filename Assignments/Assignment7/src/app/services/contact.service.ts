 import { Injectable, signal } from '@angular/core';
import { Contact } from '../models/contact.model';

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  private contactsSignal = signal<Contact[]>([
    { id: 1, name: 'Ali Raza', phone: '+92-300-1234567', email: 'ali.raza@gmail.com', gender: 'Male', address: 'Model Town, Lahore', groups: ['Friends', 'Favourites'] },
    { id: 2, name: 'Ayesha Khan', phone: '+92-333-2345678', email: 'ayesha.khan@yahoo.com', gender: 'Female', address: 'Clifton, Karachi', groups: ['Family'] },
    { id: 3, name: 'Hassan Ahmed', phone: '+92-321-3456789', email: 'hassan.ahmed@hotmail.com', gender: 'Male', address: 'G-10, Islamabad', groups: ['Classmates'] },
    { id: 4, name: 'Fatima Malik', phone: '+92-312-4567890', email: 'fatima.malik@gmail.com', gender: 'Female', address: 'Saddar, Peshawar', groups: ['Friends'] },
    { id: 5, name: 'Bilal Sheikh', phone: '+92-301-5678901', email: 'bilal.sheikh@gmail.com', gender: 'Male', address: 'Gulberg, Lahore', groups: ['Family', 'Favourites'] },
    { id: 6, name: 'Sana Iqbal', phone: '+92-344-6789012', email: 'sana.iqbal@gmail.com', gender: 'Female', address: 'F-7, Islamabad', groups: ['Friends'] },
    { id: 7, name: 'Umer Farooq', phone: '+92-300-7890123', email: 'umer.farooq@gmail.com', gender: 'Male', address: 'North Nazimabad, Karachi', groups: ['Classmates', 'Friends'] },
    { id: 8, name: 'Zara Ali', phone: '+92-311-8901234', email: 'zara.ali@yahoo.com', gender: 'Female', address: 'Satellite Town, Rawalpindi', groups: ['Favourites'] },
    { id: 9, name: 'Imran Khan', phone: '+92-322-9012345', email: 'imran.khan@gmail.com', gender: 'Male', address: 'University Road, Peshawar', groups: ['Family'] },
    { id: 10, name: 'Hina Aslam', phone: '+92-333-0123456', email: 'hina.aslam@gmail.com', gender: 'Female', address: 'Cantt, Multan', groups: ['Friends'] },
    { id: 11, name: 'Waqas Javed', phone: '+92-321-1122334', email: 'waqas.javed@gmail.com', gender: 'Male', address: 'Iqbal Town, Lahore', groups: ['Friends'] },
    { id: 12, name: 'Maryam Shah', phone: '+92-300-2233445', email: 'maryam.shah@hotmail.com', gender: 'Female', address: 'Bahria Town, Islamabad', groups: ['Family'] },
    { id: 13, name: 'Ahmed Ali', phone: '+92-301-3344556', email: 'ahmed.ali@yahoo.com', gender: 'Male', address: 'Defence, Karachi', groups: ['Classmates'] },
    { id: 14, name: 'Sara Butt', phone: '+92-333-4455667', email: 'sara.butt@gmail.com', gender: 'Female', address: 'Johar Town, Lahore', groups: ['Favourites'] },
    { id: 15, name: 'Muneeb Khan', phone: '+92-312-5566778', email: 'muneeb.khan@gmail.com', gender: 'Male', address: 'Hayatabad, Peshawar', groups: ['Friends'] },
    { id: 16, name: 'Kiran Baloch', phone: '+92-344-6677889', email: 'kiran.baloch@gmail.com', gender: 'Female', address: 'Sadar, Quetta', groups: ['Family'] },
    { id: 17, name: 'Adnan Tariq', phone: '+92-322-7788990', email: 'adnan.tariq@gmail.com', gender: 'Male', address: 'Gulshan-e-Iqbal, Karachi', groups: ['Friends'] },
    { id: 18, name: 'Rabia Noreen', phone: '+92-311-8899001', email: 'rabia.noreen@gmail.com', gender: 'Female', address: 'Satellite Town, Gujranwala', groups: ['Favourites'] },
    { id: 19, name: 'Fahad Hussain', phone: '+92-300-9900112', email: 'fahad.hussain@gmail.com', gender: 'Male', address: 'Cantt, Sialkot', groups: ['Family'] },
    { id: 20, name: 'Aqsa Jameel', phone: '+92-333-1011122', email: 'aqsa.jameel@gmail.com', gender: 'Female', address: 'Samanabad, Lahore', groups: ['Classmates'] },
    { id: 21, name: 'Nouman Qureshi', phone: '+92-301-1213141', email: 'nouman.qureshi@gmail.com', gender: 'Male', address: 'F-11, Islamabad', groups: ['Friends'] },
    { id: 22, name: 'Iqra Chaudhry', phone: '+92-321-1314151', email: 'iqra.chaudhry@gmail.com', gender: 'Female', address: 'Cantt, Faisalabad', groups: ['Family'] },
    { id: 23, name: 'Saad Anwar', phone: '+92-344-1415161', email: 'saad.anwar@gmail.com', gender: 'Male', address: 'Wapda Town, Lahore', groups: ['Friends'] },
    { id: 24, name: 'Sadia Rehman', phone: '+92-322-1516171', email: 'sadia.rehman@gmail.com', gender: 'Female', address: 'University Road, Peshawar', groups: ['Favourites'] },
    { id: 25, name: 'Tahir Nawaz', phone: '+92-311-1617181', email: 'tahir.nawaz@gmail.com', gender: 'Male', address: 'Model Town, Multan', groups: ['Family'] },
    { id: 26, name: 'Mahnoor Zahid', phone: '+92-300-1718191', email: 'mahnoor.zahid@gmail.com', gender: 'Female', address: 'Jinnah Colony, Faisalabad', groups: ['Friends'] },
    { id: 27, name: 'Shahzad Ali', phone: '+92-333-1819202', email: 'shahzad.ali@gmail.com', gender: 'Male', address: 'Defence, Lahore', groups: ['Classmates'] },
    { id: 28, name: 'Hira Siddiqui', phone: '+92-301-1920212', email: 'hira.siddiqui@gmail.com', gender: 'Female', address: 'Gulshan Colony, Gujrat', groups: ['Favourites'] },
    { id: 29, name: 'Osman Khalid', phone: '+92-321-2021222', email: 'osman.khalid@gmail.com', gender: 'Male', address: 'Defence, Karachi', groups: ['Family'] },
    { id: 30, name: 'Maliha Tariq', phone: '+92-344-2122232', email: 'maliha.tariq@gmail.com', gender: 'Female', address: 'Iqbal Town, Lahore', groups: ['Friends'] },
    { id: 31, name: 'Arslan Haider', phone: '+92-322-2223242', email: 'arslan.haider@gmail.com', gender: 'Male', address: 'Satellite Town, Rawalpindi', groups: ['Classmates'] },
    { id: 32, name: 'Nida Yasir', phone: '+92-311-2324252', email: 'nida.yasir@gmail.com', gender: 'Female', address: 'Model Town, Lahore', groups: ['Friends'] },
    { id: 33, name: 'Kamran Javed', phone: '+92-300-2425262', email: 'kamran.javed@gmail.com', gender: 'Male', address: 'Iqbal Colony, Sargodha', groups: ['Family'] },
    { id: 34, name: 'Anam Saeed', phone: '+92-333-2526272', email: 'anam.saeed@gmail.com', gender: 'Female', address: 'PECHS, Karachi', groups: ['Friends'] },
    { id: 35, name: 'Asad Bashir', phone: '+92-301-2627282', email: 'asad.bashir@gmail.com', gender: 'Male', address: 'Cantt, Abbottabad', groups: ['Classmates'] },
    { id: 36, name: 'Laiba Noor', phone: '+92-321-2728292', email: 'laiba.noor@gmail.com', gender: 'Female', address: 'Civil Lines, Gujranwala', groups: ['Favourites'] },
    { id: 37, name: 'Naveed Akhtar', phone: '+92-344-2829302', email: 'naveed.akhtar@gmail.com', gender: 'Male', address: 'Railway Colony, Multan', groups: ['Family'] },
    { id: 38, name: 'Mehwish Ali', phone: '+92-322-2930312', email: 'mehwish.ali@gmail.com', gender: 'Female', address: 'Bahria Town, Lahore', groups: ['Friends'] },
    { id: 39, name: 'Shahbaz Khan', phone: '+92-311-3031322', email: 'shahbaz.khan@gmail.com', gender: 'Male', address: 'University Town, Peshawar', groups: ['Friends'] },
    { id: 40, name: 'Mariam Iqbal', phone: '+92-300-3132332', email: 'mariam.iqbal@gmail.com', gender: 'Female', address: 'Clifton, Karachi', groups: ['Favourites'] },
    { id: 41, name: 'Farhan Rasheed', phone: '+92-333-3233342', email: 'farhan.rasheed@gmail.com', gender: 'Male', address: 'Model Town, Lahore', groups: ['Classmates'] },
    { id: 42, name: 'Haleema Khan', phone: '+92-301-3334352', email: 'haleema.khan@gmail.com', gender: 'Female', address: 'Cantt, Rawalpindi', groups: ['Friends'] },
    { id: 43, name: 'Danish Saleem', phone: '+92-321-3435362', email: 'danish.saleem@gmail.com', gender: 'Male', address: 'Johar Town, Lahore', groups: ['Family'] },
    { id: 44, name: 'Amina Shahid', phone: '+92-344-3536372', email: 'amina.shahid@gmail.com', gender: 'Female', address: 'Garden Town, Faisalabad', groups: ['Friends'] },
    { id: 45, name: 'Zeeshan Haq', phone: '+92-322-3637382', email: 'zeeshan.haq@gmail.com', gender: 'Male', address: 'Satellite Town, Quetta', groups: ['Family'] },
    { id: 46, name: 'Bushra Waheed', phone: '+92-311-3738392', email: 'bushra.waheed@gmail.com', gender: 'Female', address: 'Shalimar Town, Lahore', groups: ['Friends'] },
    { id: 47, name: 'Imtiaz Gondal', phone: '+92-300-3839402', email: 'imtiaz.gondal@gmail.com', gender: 'Male', address: 'Civil Lines, Sialkot', groups: ['Classmates'] },
    { id: 48, name: 'Sadia Khursheed', phone: '+92-333-3940412', email: 'sadia.khursheed@gmail.com', gender: 'Female', address: 'Saddar, Karachi', groups: ['Friends'] },
    { id: 49, name: 'Shafiq Anjum', phone: '+92-301-4041422', email: 'shafiq.anjum@gmail.com', gender: 'Male', address: 'Cantt, Lahore', groups: ['Family'] },
    { id: 50, name: 'Nimra Zafar', phone: '+92-321-4142432', email: 'nimra.zafar@gmail.com', gender: 'Female', address: 'Defence, Islamabad', groups: ['Favourites'] }
   ]);

  get contacts() {
    return this.contactsSignal.asReadonly();
  }

  updateContact(contact: Contact): void {
    this.contactsSignal.update(contacts => {
      const index = contacts.findIndex(c => c.id === contact.id);
      if (index !== -1) {
        const updated = [...contacts];
        updated[index] = { ...contact };
        return updated;
      }
      return contacts;
    });
  }
}
