using System;
using System.Collections.Generic;
using System.Text;

namespace umisyDeneme_winform
{

   

    public class Urun
    {
        public string  Barkod = "";

        public  int konumNum = 0;

        public bool redDurumu = false;

        public int Burossa_Aparat_No_1 = 0;

        public  KalınlıkKontrol boyasızKalınlıkKontrol = new KalınlıkKontrol();

        public  DefectDegerler boyasızDefectDegerler = new DefectDegerler();

        public  TıpaKontrol TıpaKontrolVeri = new TıpaKontrol();

        public  KalınlıkKontrol BoyalıKalınlıkKOntrol = new KalınlıkKontrol();

        public manuelDefectDegerler manuelKalınlık = new manuelDefectDegerler();

        public  DefectDegerler BoyalıDefectKontrol = new DefectDegerler();

        public  Robotkontrol KrtRobotKontrol = new Robotkontrol();

        public  TorkDegerler TorkDegerler = new TorkDegerler();

        public   DateTime urunKoymaZamani;
        public  DateTime Krt1TaramaBitmeZamani;
        public  DateTime Kvy7OnayGeldiZamani;
        public  DateTime TıpaKontrolZamani;
        public  DateTime Kvy8AsmaTalebiZamani;
        public  DateTime Kvy14AskidanUrunİndiZamani;
        public  DateTime Krt2TaramaBittiZamani;
        public DateTime Kvy1618OnayRedZamani;
        public  DateTime RobotKontrolBitirdiZamani;
        public  DateTime Kvy9KasalaSinyaliVerdiZamani;
        public  DateTime HattaGirisZamani;
        public DateTime krt_kontrol_tarih_1;
        public DateTime krt_kontrol_tarih_2;
        public DateTime krt_kontrol_tarih_3;

        public bool Krt_kontrol_onay_1;
        public bool Krt_kontrol_onay_2;
        public bool Krt_kontrol_onay_3;

        public bool rework_secimi = false; // 1 ise rework1 0 ise rework2

    }

    public class KalınlıkKontrol
    {
       public  int kalınlikDeger1 = 0;
        public int kalınlikDeger2 = 0;
        public int kalınlikDeger3 = 0;
        public int kalınlikDeger4 = 0;

        
    }

    public class DefectDegerler
    {
        public int defectDeger1 = 0;
        public int defectDeger2 = 0;
        public int defectDeger3 = 0;
        public int defectDeger4 = 0;
        public int defectDeger5 = 0;
        public int defectDeger6 = 0;
        public int defectDeger7 = 0;
        public int defectDeger8 = 0;


    }

    public class manuelDefectDegerler
    {
        public int manuelkalınlıkDeger1 = 0;
            public int manuelkalınlıkDeger2 = 0;
                public int manuelkalınlıkDeger3 = 0;
                public int manuelkalınlıkDeger4 = 0;
                public int manuelkalınlıkDeger5 = 0;
                    public int manuelkalınlıkDeger6 = 0;
                    public int manuelkalınlıkDeger7 = 0;
                    public int manuelkalınlıkDeger8 = 0;

        
    }

    public class TıpaKontrol
    {
        public bool tıpaVarYok1 = false;
        public bool tıpaVarYok2 = false;
        public bool tıpaVarYok3 = false;
        public bool tıpaVarYok4 = false;
        public bool tıpaVarYok5 = false;
        public bool tıpaVarYok6 = false;
        public bool tıpaVarYok7 = false;
        public bool tıpaVarYok8 = false;
        public bool tıpaVarYok9 = false;
        public bool tıpaVarYok10 = false;
        public bool tıpaVarYok11 = false;
        public bool tıpaVarYok12 = false;
        public bool tıpaVarYok13 = false;
        public bool tıpaVarYok14 = false;
        public bool tıpaVarYok15 = false;

        public bool tıpaVarYok16 = false;
        public bool tıpaVarYok17 = false;
        public bool tıpaVarYok18 = false;

        
    }

    public class Robotkontrol
    {
        public bool RobotKontrol1 = false;
        public bool RobotKontrol2 = false;
        public bool RobotKontrol3 = false;
        public bool RobotKontrol4 = false;
        public bool RobotKontrol5 = false;
        public bool RobotKontrol6 = false;
        public bool RobotKontrol7 = false;
        public bool RobotKontrol8 = false;
        public bool RobotKontrol9 = false;
        public bool RobotKontrol10 = false;
        public bool RobotKontrol11 = false;
        public bool RobotKontrol12 = false;
        public bool RobotKontrol13 = false;




       
    }

    public class TorkDegerler
    {
        public int torkDeger1 = 0;
        public int torkDeger2 = 0;
        public int torkDeger3 = 0;
            public int torkDeger4 = 0;
       
    }



}
