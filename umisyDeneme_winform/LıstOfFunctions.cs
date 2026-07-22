using Microsoft.Data.SqlClient;
using S7.Net;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace umisyDeneme_winform
{

    public static class LıstOfFunctions
    {
        //string okuma
        public static string PlcStringOku(Plc plc, int dbNumber, int startOffset, int maxLength)
        {
            // STRING = 2 byte header + data
            byte[] buffer = plc.ReadBytes(DataType.DataBlock, dbNumber, startOffset, maxLength + 2);

            int actualLength = buffer[1]; // mevcut uzunluk

            return System.Text.Encoding.ASCII.GetString(buffer, 2, actualLength);
        }



        //ınt okuma
        public static short PlcIntOku(Plc? plc, int? dbNumber, int? startOffset)
        {
            short deger = (short)plc.Read($"DB{dbNumber}.DBW{startOffset}");
            return deger;
        }



        //real okuma
        public static double PlcLrealOku(Plc plc, int dbNumberi, int startOffset)
        {
            double deger = (double)plc.Read($"DB{dbNumberi}.DBD{startOffset}");
            return deger;
        }



        //sint okuma
        public static sbyte PlcSintOku(Plc plc, int dbNumber, int startOffset)
        {
            sbyte deger = (sbyte)(byte)plc.Read($"DB{dbNumber}.DBB{startOffset}");
            return deger;
        }


        //boolean okuma
        public static bool PlcBooleanOku(Plc plc, int dbNumber, int startOffset, int bitNumber)
        {
            bool deger = (bool)plc.Read($"DB{dbNumber}.DBX{startOffset}.{bitNumber}");
            return deger;
        }

        //dint okuma
        public static int PlcDintOku(Plc plc, int dbNumber, int startOffset)
        {
            // DINT = 4 byte → DBD
            var value = 0;
            object raw = plc.Read($"DB{dbNumber}.DBD{startOffset}");
            if (raw is int i)
            {
                value = i;
            }
            else if (raw is uint u)
            {
                value = unchecked((int)u); // -2 gibi değerleri düzeltir
            }
            else
            {
                value = Convert.ToInt32(raw);
            }
            return value;
        }

        //lreal okuma
        public static int PlcRealOku(Plc plc, int dbNumber, int startOffset)
        {
            byte[] buffer = plc.ReadBytes(DataType.DataBlock, dbNumber, startOffset, 4);

            // Siemens byte swap fix
            Array.Reverse(buffer);

            float value = BitConverter.ToSingle(buffer, 0);
            int scaled = (int)Math.Round(value * 10000);
            return scaled;
        }

        //datetime okuma
        public static System.DateTime PlcDateTimeOkuSafe(Plc plc, int dbNumber, int startOffset)
        {
            try
            {
                byte[] buffer = plc.ReadBytes(DataType.DataBlock, dbNumber, startOffset, 8);

                int year = BcdToInt(buffer[0]);
                int month = BcdToInt(buffer[1]);
                int day = BcdToInt(buffer[2]);
                int hour = BcdToInt(buffer[3]);
                int minute = BcdToInt(buffer[4]);
                int second = BcdToInt(buffer[5]);

                int msHigh = BcdToInt(buffer[6]);
                int msLow = BcdToInt((byte)(buffer[7] >> 4));
                int millisecond = msHigh * 10 + msLow;

                // Siemens yıl 90-99 → 1900, 00-89 → 2000
                year += (year >= 90) ? 1900 : 2000;

                // Geçerli aralık kontrolü
                if (month < 1 || month > 12) month = 1;
                if (day < 1 || day > 31) day = 1;
                if (hour > 23) hour = 0;
                if (minute > 59) minute = 0;
                if (second > 59) second = 0;
                if (millisecond > 999) millisecond = 0;

                return new System.DateTime(year, month, day, hour, minute, second, millisecond);
            }
            catch
            {
                return new System.DateTime(); // Hatalı veri varsa null döner
            }
        }

        private static int BcdToInt(byte b)
        {
            return ((b >> 4) * 10) + (b & 0x0F);
        }

        //var olan barkod sorgusu
        public static bool BarkodVarMi(string barkod, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT COUNT(1) FROM UmisyMakineOtomasyonVeriTablosu WHERE Barkod = @barkod";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barkod", barkod);

                    int count = (int)cmd.ExecuteScalar();

                    return count == 0; // varsa true
                }
            }
        }



        //kvy 9 veri okuma
        public static umisyDeneme_winform.Urun PlcKVY9Oku(Plc? plc)
        {
            umisyDeneme_winform.Urun urun = new umisyDeneme_winform.Urun();



            //parça barkodu
            urun.Barkod = umisyDeneme_winform.LıstOfFunctions.PlcStringOku(plc, 149, 8296, 50);

            //ParçaKonum numarası
            urun.konumNum = umisyDeneme_winform.LıstOfFunctions.PlcSintOku(plc, 149, 8552);

            //parça red Durumu
            urun.redDurumu = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 8553, 0);





            //boyalı kalınlık kontrol
            urun.BoyalıKalınlıkKOntrol.kalınlikDeger1 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8932);
            urun.BoyalıKalınlıkKOntrol.kalınlikDeger2 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8936);
            urun.BoyalıKalınlıkKOntrol.kalınlikDeger3 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8940);
            urun.BoyalıKalınlıkKOntrol.kalınlikDeger4 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8944);


            //boyali manuel kalınlık kontrol
            urun.manuelKalınlık.manuelkalınlıkDeger1 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8948);
            urun.manuelKalınlık.manuelkalınlıkDeger2 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8952);
            urun.manuelKalınlık.manuelkalınlıkDeger3 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8956);
            urun.manuelKalınlık.manuelkalınlıkDeger4 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8960);
            urun.manuelKalınlık.manuelkalınlıkDeger5 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8964);
            urun.manuelKalınlık.manuelkalınlıkDeger6 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8968);
            urun.manuelKalınlık.manuelkalınlıkDeger7 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8972);
            urun.manuelKalınlık.manuelkalınlıkDeger8 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8976);

            //boyali defect değerler

            urun.BoyalıDefectKontrol.defectDeger1 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 8992);
            urun.BoyalıDefectKontrol.defectDeger2 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 9000);
            urun.BoyalıDefectKontrol.defectDeger3 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 9008);
            urun.BoyalıDefectKontrol.defectDeger4 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 9016);
            urun.BoyalıDefectKontrol.defectDeger5 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 9024);
            urun.BoyalıDefectKontrol.defectDeger6 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 9032);
            urun.BoyalıDefectKontrol.defectDeger7 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 9040);
            urun.BoyalıDefectKontrol.defectDeger8 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 9048);


            //robot kontrol
            urun.KrtRobotKontrol.RobotKontrol1 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9142, 0);
            urun.KrtRobotKontrol.RobotKontrol2 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9142, 1);
            urun.KrtRobotKontrol.RobotKontrol3 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9142, 2);
            urun.KrtRobotKontrol.RobotKontrol4 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9142, 3);
            urun.KrtRobotKontrol.RobotKontrol5 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9142, 4);
            urun.KrtRobotKontrol.RobotKontrol6 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9142, 5);
            urun.KrtRobotKontrol.RobotKontrol7 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9142, 6);
            urun.KrtRobotKontrol.RobotKontrol8 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9142, 7);
            urun.KrtRobotKontrol.RobotKontrol9 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9143, 0);
            urun.KrtRobotKontrol.RobotKontrol10 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9143, 1);
            urun.KrtRobotKontrol.RobotKontrol11 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9143, 2);
            urun.KrtRobotKontrol.RobotKontrol12 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9143, 3);
            urun.KrtRobotKontrol.RobotKontrol13 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9143, 4);



            //zamanlar

            urun.Kvy14AskidanUrunİndiZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 9200);
            urun.Krt2TaramaBittiZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 9208);
            urun.Kvy1618OnayRedZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 9216);
            urun.RobotKontrolBitirdiZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 9224);
            urun.Kvy9KasalaSinyaliVerdiZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 9232);
            urun.HattaGirisZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 9264);

            //krtkontrol

            urun.krt_kontrol_tarih_1 = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 9274);
            urun.krt_kontrol_tarih_2 = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 9282);
            urun.krt_kontrol_tarih_3 = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 9290);

            urun.Krt_kontrol_onay_1 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9298, 0);
            urun.Krt_kontrol_onay_2 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9298, 1);
            urun.Krt_kontrol_onay_3 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9298, 2);
            urun.rework_secimi = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 9272, 0);




            return urun;
        }

        //kvy 8 veri okuma

        public static umisyDeneme_winform.Urun PlcKVY8Oku(Plc? plc)
        {
            umisyDeneme_winform.Urun urun = new umisyDeneme_winform.Urun();



            //parça barkodu
            urun.Barkod = umisyDeneme_winform.LıstOfFunctions.PlcStringOku(plc, 149, 7260, 50);

            //ParçaKonum numarası
            urun.konumNum = umisyDeneme_winform.LıstOfFunctions.PlcSintOku(plc, 149, 7516);

            //parça red Durumu
            urun.redDurumu = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7517, 0);

            //boyasız kalınlık kontrol
            urun.boyasızKalınlıkKontrol.kalınlikDeger1 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7518);
            urun.boyasızKalınlıkKontrol.kalınlikDeger2 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7522);
            urun.boyasızKalınlıkKontrol.kalınlikDeger3 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7526);
            urun.boyasızKalınlıkKontrol.kalınlikDeger4 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7530);

            //boyasız defect kontrol
            urun.boyasızDefectDegerler.defectDeger1 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7542);
            urun.boyasızDefectDegerler.defectDeger2 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7550);
            urun.boyasızDefectDegerler.defectDeger3 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7558);
            urun.boyasızDefectDegerler.defectDeger4 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7566);
            urun.boyasızDefectDegerler.defectDeger5 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7574);
            urun.boyasızDefectDegerler.defectDeger6 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7582);
            urun.boyasızDefectDegerler.defectDeger7 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7590);
            urun.boyasızDefectDegerler.defectDeger8 = umisyDeneme_winform.LıstOfFunctions.PlcDintOku(plc, 149, 7598);


            //TıpaKontrol
            urun.TıpaKontrolVeri.tıpaVarYok1 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7892, 0);
            urun.TıpaKontrolVeri.tıpaVarYok2 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7892, 1);
            urun.TıpaKontrolVeri.tıpaVarYok3 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7892, 2);
            urun.TıpaKontrolVeri.tıpaVarYok4 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7892, 3);
            urun.TıpaKontrolVeri.tıpaVarYok5 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7892, 4);
            urun.TıpaKontrolVeri.tıpaVarYok6 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7892, 5);
            urun.TıpaKontrolVeri.tıpaVarYok7 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7892, 6);
            urun.TıpaKontrolVeri.tıpaVarYok8 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7892, 7);
            urun.TıpaKontrolVeri.tıpaVarYok9 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7893, 0);
            urun.TıpaKontrolVeri.tıpaVarYok10 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7893, 1);
            urun.TıpaKontrolVeri.tıpaVarYok11 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7893, 2);
            urun.TıpaKontrolVeri.tıpaVarYok12 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7893, 3);
            urun.TıpaKontrolVeri.tıpaVarYok13 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7893, 4);
            urun.TıpaKontrolVeri.tıpaVarYok14 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7893, 5);
            urun.TıpaKontrolVeri.tıpaVarYok15 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7893, 6);
            urun.TıpaKontrolVeri.tıpaVarYok16 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7893, 7);
            urun.TıpaKontrolVeri.tıpaVarYok17 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7894, 0);
            urun.TıpaKontrolVeri.tıpaVarYok18 = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7894, 1);




            //torkDegerleri
            urun.TorkDegerler.torkDeger1 = umisyDeneme_winform.LıstOfFunctions.PlcRealOku(plc, 149, 8108);
            urun.TorkDegerler.torkDeger2 = umisyDeneme_winform.LıstOfFunctions.PlcRealOku(plc, 149, 8112);
            urun.TorkDegerler.torkDeger3 = umisyDeneme_winform.LıstOfFunctions.PlcRealOku(plc, 149, 8116);
            urun.TorkDegerler.torkDeger4 = umisyDeneme_winform.LıstOfFunctions.PlcRealOku(plc, 149, 8120);

            //zamanlar
            urun.urunKoymaZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 8124);
            urun.Krt1TaramaBitmeZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 8132);
            urun.Kvy7OnayGeldiZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 8140);
            urun.TıpaKontrolZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 8148);
            urun.Kvy8AsmaTalebiZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 8156);
            urun.Kvy14AskidanUrunİndiZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 8164);
            urun.Krt2TaramaBittiZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 8172);
            urun.Kvy1618OnayRedZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 8180);
            urun.RobotKontrolBitirdiZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 8188);
            urun.Kvy9KasalaSinyaliVerdiZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 8196);
            urun.HattaGirisZamani = umisyDeneme_winform.LıstOfFunctions.PlcDateTimeOkuSafe(plc, 149, 8228);




            return urun;
        }



        // sql veri yazma 
        public static void VeriYaz(Plc plc, string connStr, string connstr_burosso)
        {
            try
            {
                plc.Open();

                if (plc.IsConnected)
                {

                    var kvy9UrunVar = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 8050, 7);
                    if (kvy9UrunVar)
                    {

                        Urun urundegerlerı = umisyDeneme_winform.LıstOfFunctions.PlcKVY9Oku(plc);





                        if (umisyDeneme_winform.LıstOfFunctions.BarkodVarMi(urundegerlerı.Barkod, connStr))
                        {
                            using (SqlConnection conn = new SqlConnection(connStr))
                            {
                                conn.Open();

                                string query = @"
                                            INSERT  INTO XPODA.dbo.UmisyHatVeriTablosu
                                            (
                                                TARIH,
                                                BARA_NUMARASI,
                                                BARKOD,
                                                
                                                PARCA_DURUM,
                                                BOYAZSIZ_DEFECT1,
                                                BOYAZSIZ_DEFECT2,
                                                BOYAZSIZ_DEFECT3,
                                                BOYAZSIZ_DEFECT4,
                                                BOYAZSIZ_DEFECT5,
                                                BOYAZSIZ_DEFECT6,
                                                BOYAZSIZ_DEFECT7,
                                                BOYAZSIZ_DEFECT8,

                                                BOYA_KALINLIK1,
                                                BOYA_KALINLIK2,
                                                BOYA_KALINLIK3,
                                                BOYA_KALINLIK4,

                                                MASKE_KONTROL_1,
                                                MASKE_KONTROL_2,
                                                MASKE_KONTROL_3,
                                                MASKE_KONTROL_4,
                                                MASKE_KONTROL_5,
                                                MASKE_KONTROL_6,
                                                MASKE_KONTROL_7,
                                                MASKE_KONTROL_8,
                                                MASKE_KONTROL_9,
                                                MASKE_KONTROL_10,
                                                MASKE_KONTROL_11,
                                                MASKE_KONTROL_12,
                                                MASKE_KONTROL_13,

                                                BOYALI_DEFECT_1,
                                                BOYALI_DEFECT_2,
                                                BOYALI_DEFECT_3,
                                                BOYALI_DEFECT_4,
                                                BOYALI_DEFECT_5,
                                                BOYALI_DEFECT_6,
                                                BOYALI_DEFECT_7,
                                                BOYALI_DEFECT_8,

                                                MANUEL_KALINLIK_1,
                                                MANUEL_KALINLIK_2,
                                                MANUEL_KALINLIK_3,
                                                MANUEL_KALINLIK_4,
                                                MANUEL_KALINLIK_5,
                                                MANUEL_KALINLIK_6,
                                                MANUEL_KALINLIK_7,
                                                MANUEL_KALINLIK_8,

                                                ROBOT_KONTROL_ONKARE,
                                                ROBOT_KONTROL_SP1,
                                                ROBOT_KONTROL_SP2,
                                                ROBOT_KONTROL_SP3,
                                                ROBOT_KONTROL_SP4,
                                                ARKA_ALTILI_1,
                                                ARKA_ALTILI_2,
                                                ARKA_ALTILI_3,
                                                ARKA_ALTILI_4,
                                                ARKA_ALTILI_5,
                                                ARKA_ALTILI_6,
                                                ARKA_KARE,	
                                                ARKA_KARE_CEVRE,

                                                TORK_DEGERI_1,
                                                TORK_DEGERI_2,
                                                TORK_DEGERI_3,
                                                TORK_DEGERI_4,

                                                HAT_GIRIS_ZAMAN,
                                                ISLEM_BITIS_ZAMAN1,
                                                ISLEM_BITIS_ZAMAN2,
                                                ISLEM_BITIS_ZAMAN3,
                                                ISLEM_BITIS_ZAMAN4,
                                                ISLEM_BITIS_ZAMAN5,
                                                ISLEM_BITIS_ZAMAN6,
                                                ISLEM_BITIS_ZAMAN7,
                                                ISLEM_BITIS_ZAMAN8,
                                                ISLEM_BITIS_ZAMAN9,
                                                ISLEM_BITIS_ZAMAN10


                     




                                            )
                                            VALUES
                                            (
                                                @TARIH,
                                                @BARA_NUMARASI,
                                                @BARKOD,
                                                
                                                @PARCA_DURUM,
                                                @BOYASIZ_DEFECT1,
                                                @BOYASIZ_DEFECT2,
                                                @BOYASIZ_DEFECT3,
                                                @BOYASIZ_DEFECT4,
                                                @BOYASIZ_DEFECT5,
                                                @BOYASIZ_DEFECT6,
                                                @BOYASIZ_DEFECT7,
                                                @BOYASIZ_DEFECT8,

                                                @BOYA_KALINLIK_1,
                                                @BOYA_KALINLIK_2,
                                                @BOYA_KALINLIK_3,
                                                @BOYA_KALINLIK_4,

                                                @MASKE_KONTROL_1,
                                                @MASKE_KONTROL_2,
                                                @MASKE_KONTROL_3,
                                                @MASKE_KONTROL_4,
                                                @MASKE_KONTROL_5,
                                                @MASKE_KONTROL_6,
                                                @MASKE_KONTROL_7,
                                                @MASKE_KONTROL_8,
                                                @MASKE_KONTROL_9,
                                                @MASKE_KONTROL_10,
                                                @MASKE_KONTROL_11,
                                                @MASKE_KONTROL_12,
                                                @MASKE_KONTROL_13,

                                                @BOYALI_DEFECT_1,
                                                @BOYALI_DEFECT_2,
                                                @BOYALI_DEFECT_3,
                                                @BOYALI_DEFECT_4,
                                                @BOYALI_DEFECT_5,
                                                @BOYALI_DEFECT_6,
                                                @BOYALI_DEFECT_7,
                                                @BOYALI_DEFECT_8,

                                                @MANUEL_KALINLIK_1,
                                                @MANUEL_KALINLIK_2,
                                                @MANUEL_KALINLIK_3,
                                                @MANUEL_KALINLIK_4,
                                                @MANUEL_KALINLIK_5,
                                                @MANUEL_KALINLIK_6,
                                                @MANUEL_KALINLIK_7,
                                                @MANUEL_KALINLIK_8,

                                                @ROBOT_KONTROL_ONKARE,
                                                @ROBOT_KONTROL_SP1,
                                                @ROBOT_KONTROL_SP2,
                                                @ROBOT_KONTROL_SP3,
                                                @ROBOT_KONTROL_SP4,
                                                @ARKA_ALTILI_1,
                                                @ARKA_ALTILI_2,
                                                @ARKA_ALTILI_3,
                                                @ARKA_ALTILI_4,
                                                @ARKA_ALTILI_5,
                                                @ARKA_ALTILI_6,
                                                @ARKA_KARE,	
                                                @ARKA_KARE_CEVRE,

                                                @TORK_DEGERI_1,
                                                @TORK_DEGERI_2,
                                                @TORK_DEGERI_3,
                                                @TORK_DEGERI_4,

                                                @HAT_GIRIS_ZAMAN,

                                                @ISLEM_BITIS_ZAMAN1,
                                                @ISLEM_BITIS_ZAMAN2,
                                                @ISLEM_BITIS_ZAMAN3,
                                                @ISLEM_BITIS_ZAMAN4,
                                                @ISLEM_BITIS_ZAMAN5,
                                                @ISLEM_BITIS_ZAMAN6,
                                                @ISLEM_BITIS_ZAMAN7,
                                                @ISLEM_BITIS_ZAMAN8,
                                                @ISLEM_BITIS_ZAMAN9,
                                                @ISLEM_BITIS_ZAMAN10
                                            )";

                                using (SqlCommand cmd = new SqlCommand(query, conn))
                                {

                                    cmd.Parameters.AddWithValue("@TARIH", System.DateTime.UtcNow);

                                    cmd.Parameters.AddWithValue("@BARA_NUMARASI", 12);

                                    cmd.Parameters.AddWithValue("@BARKOD", urundegerlerı.Barkod);
                                    cmd.Parameters.AddWithValue("@PARCA_DURUM", urundegerlerı.redDurumu);   // bit
                                    cmd.Parameters.AddWithValue("@BOYASIZ_DEFECT1", (int)(urundegerlerı.boyasızDefectDegerler.defectDeger1));
                                    cmd.Parameters.AddWithValue("@BOYASIZ_DEFECT2", (int)(urundegerlerı.boyasızDefectDegerler.defectDeger2));
                                    cmd.Parameters.AddWithValue("@BOYASIZ_DEFECT3", (int)urundegerlerı.boyasızDefectDegerler.defectDeger3);
                                    cmd.Parameters.AddWithValue("@BOYASIZ_DEFECT4", (int)urundegerlerı.boyasızDefectDegerler.defectDeger4);
                                    cmd.Parameters.AddWithValue("@BOYASIZ_DEFECT5", (int)urundegerlerı.boyasızDefectDegerler.defectDeger5);
                                    cmd.Parameters.AddWithValue("@BOYASIZ_DEFECT6", (int)urundegerlerı.boyasızDefectDegerler.defectDeger6);
                                    cmd.Parameters.AddWithValue("@BOYASIZ_DEFECT7", (int)urundegerlerı.boyasızDefectDegerler.defectDeger7);
                                    cmd.Parameters.AddWithValue("@BOYASIZ_DEFECT8", (int)urundegerlerı.boyasızDefectDegerler.defectDeger8);


                                    cmd.Parameters.AddWithValue("@BOYA_KALINLIK_1", (int)urundegerlerı.BoyalıKalınlıkKOntrol.kalınlikDeger1 - (int)urundegerlerı.boyasızKalınlıkKontrol.kalınlikDeger1);
                                    cmd.Parameters.AddWithValue("@BOYA_KALINLIK_2", (int)urundegerlerı.BoyalıKalınlıkKOntrol.kalınlikDeger2 - (int)urundegerlerı.boyasızKalınlıkKontrol.kalınlikDeger2);
                                    cmd.Parameters.AddWithValue("@BOYA_KALINLIK_3", (int)urundegerlerı.BoyalıKalınlıkKOntrol.kalınlikDeger3 - (int)urundegerlerı.boyasızKalınlıkKontrol.kalınlikDeger3);
                                    cmd.Parameters.AddWithValue("@BOYA_KALINLIK_4", (int)urundegerlerı.BoyalıKalınlıkKOntrol.kalınlikDeger4 - (int)urundegerlerı.boyasızKalınlıkKontrol.kalınlikDeger4);


                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_1", urundegerlerı.TıpaKontrolVeri.tıpaVarYok1);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_2", urundegerlerı.TıpaKontrolVeri.tıpaVarYok2);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_3", urundegerlerı.TıpaKontrolVeri.tıpaVarYok3);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_4", urundegerlerı.TıpaKontrolVeri.tıpaVarYok4);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_5", urundegerlerı.TıpaKontrolVeri.tıpaVarYok5);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_6", urundegerlerı.TıpaKontrolVeri.tıpaVarYok6);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_7", urundegerlerı.TıpaKontrolVeri.tıpaVarYok7);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_8", urundegerlerı.TıpaKontrolVeri.tıpaVarYok8);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_9", urundegerlerı.TıpaKontrolVeri.tıpaVarYok9);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_10", urundegerlerı.TıpaKontrolVeri.tıpaVarYok10);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_11", urundegerlerı.TıpaKontrolVeri.tıpaVarYok11);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_12", urundegerlerı.TıpaKontrolVeri.tıpaVarYok12);
                                    cmd.Parameters.AddWithValue("@MASKE_KONTROL_13", urundegerlerı.TıpaKontrolVeri.tıpaVarYok13);

                                    cmd.Parameters.AddWithValue("@BOYALI_DEFECT_1", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger1);
                                    cmd.Parameters.AddWithValue("@BOYALI_DEFECT_2", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger2);
                                    cmd.Parameters.AddWithValue("@BOYALI_DEFECT_3", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger3);
                                    cmd.Parameters.AddWithValue("@BOYALI_DEFECT_4", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger4);
                                    cmd.Parameters.AddWithValue("@BOYALI_DEFECT_5", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger5);
                                    cmd.Parameters.AddWithValue("@BOYALI_DEFECT_6", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger6);
                                    cmd.Parameters.AddWithValue("@BOYALI_DEFECT_7", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger7);
                                    cmd.Parameters.AddWithValue("@BOYALI_DEFECT_8", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger8);

                                    cmd.Parameters.AddWithValue("@MANUEL_KALINLIK_1", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger1);
                                    cmd.Parameters.AddWithValue("@MANUEL_KALINLIK_2", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger2);
                                    cmd.Parameters.AddWithValue("@MANUEL_KALINLIK_3", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger3);
                                    cmd.Parameters.AddWithValue("@MANUEL_KALINLIK_4", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger4);
                                    cmd.Parameters.AddWithValue("@MANUEL_KALINLIK_5", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger5);
                                    cmd.Parameters.AddWithValue("@MANUEL_KALINLIK_6", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger6);
                                    cmd.Parameters.AddWithValue("@MANUEL_KALINLIK_7", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger7);
                                    cmd.Parameters.AddWithValue("@MANUEL_KALINLIK_8", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger8);

                                    cmd.Parameters.AddWithValue("@ROBOT_KONTROL_ONKARE", urundegerlerı.KrtRobotKontrol.RobotKontrol1);
                                    cmd.Parameters.AddWithValue("@ROBOT_KONTROL_SP1", urundegerlerı.KrtRobotKontrol.RobotKontrol2);
                                    cmd.Parameters.AddWithValue("@ROBOT_KONTROL_SP2", urundegerlerı.KrtRobotKontrol.RobotKontrol3);
                                    cmd.Parameters.AddWithValue("@ROBOT_KONTROL_SP3", urundegerlerı.KrtRobotKontrol.RobotKontrol4);
                                    cmd.Parameters.AddWithValue("@ROBOT_KONTROL_SP4", urundegerlerı.KrtRobotKontrol.RobotKontrol5);

                                    cmd.Parameters.AddWithValue("@ARKA_ALTILI_1", urundegerlerı.KrtRobotKontrol.RobotKontrol6);
                                    cmd.Parameters.AddWithValue("@ARKA_ALTILI_2", urundegerlerı.KrtRobotKontrol.RobotKontrol7);
                                    cmd.Parameters.AddWithValue("@ARKA_ALTILI_3", urundegerlerı.KrtRobotKontrol.RobotKontrol8);
                                    cmd.Parameters.AddWithValue("@ARKA_ALTILI_4", urundegerlerı.KrtRobotKontrol.RobotKontrol9);
                                    cmd.Parameters.AddWithValue("@ARKA_ALTILI_5", urundegerlerı.KrtRobotKontrol.RobotKontrol10);
                                    cmd.Parameters.AddWithValue("@ARKA_ALTILI_6", urundegerlerı.KrtRobotKontrol.RobotKontrol11);
                                    cmd.Parameters.AddWithValue("@ARKA_KARE", urundegerlerı.KrtRobotKontrol.RobotKontrol12);
                                    cmd.Parameters.AddWithValue("@ARKA_KARE_CEVRE", urundegerlerı.KrtRobotKontrol.RobotKontrol13);

                                    cmd.Parameters.AddWithValue("@TORK_DEGERI_1", (int)urundegerlerı.TorkDegerler.torkDeger1);
                                    cmd.Parameters.AddWithValue("@TORK_DEGERI_2", (int)urundegerlerı.TorkDegerler.torkDeger2);
                                    cmd.Parameters.AddWithValue("@TORK_DEGERI_3", (int)urundegerlerı.TorkDegerler.torkDeger3);
                                    cmd.Parameters.AddWithValue("@TORK_DEGERI_4", (int)urundegerlerı.TorkDegerler.torkDeger4);

                                    cmd.Parameters.AddWithValue("@HAT_GIRIS_ZAMAN", urundegerlerı.HattaGirisZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN1", urundegerlerı.urunKoymaZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN2", urundegerlerı.Krt1TaramaBitmeZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN3", urundegerlerı.Kvy7OnayGeldiZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN4", urundegerlerı.TıpaKontrolZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN5", urundegerlerı.Kvy8AsmaTalebiZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN6", urundegerlerı.Kvy14AskidanUrunİndiZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN7", urundegerlerı.Krt2TaramaBittiZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN8", urundegerlerı.Kvy1618OnayRedZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN9", urundegerlerı.RobotKontrolBitirdiZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN10", urundegerlerı.Kvy9KasalaSinyaliVerdiZamani);




                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }







                    }

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
            }
            finally
            {
                plc.Close();
            }


        }

        public static void boyasizVeri_yaz(Plc plc, string connStr)
        {
           
                plc.Open();

                if (plc.IsConnected)
                {

                    var kvy8UrunVar = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 7254, 7);
             
                    if (kvy8UrunVar )
                    {

                        Urun urundegerlerı = umisyDeneme_winform.LıstOfFunctions.PlcKVY8Oku(plc);





                        if (umisyDeneme_winform.LıstOfFunctions.BarkodVarMi(urundegerlerı.Barkod, connStr))
                        {
                            using (SqlConnection conn = new SqlConnection(connStr))
                            {
                                conn.Open();

                                string query = @"
                                            INSERT  INTO XPODA.dbo.UmisyMakineOtomasyonVeriTablosu
                                            (
                                                id,
                                                Kayit_Tarihi,
                                                Burossa_Aparat_No_1,
                                                Barkod,
                                                
                                                Parca_Durumu,
                                                Yuzey_Puruz_Rz30_1_boyasiz,
                                                Yuzey_Puruz_Rz30_2,
                                                Yuzey_Puruz_Rz30_3,
                                                Yuzey_Puruz_Rz30_4,
                                                Yuzey_Puruz_Rz30_5,
                                                Yuzey_Puruz_Rz30_6,
                                                Yuzey_Puruz_Rz30_7,
                                                Yuzey_Puruz_Rz30_8,

                                                Kaplama_Kalinlik1,
                                                Kaplama_Kalinlik2,
                                                Kaplama_Kalinlik3,
                                                Kaplama_Kalinlik4,

                                                KM_Kara_Maske,
                                                CM_Celtikli_Maske,
                                                ST18_01_Spigot_Maske,
                                                ST22_01_Spigot_Maske,
                                                HM01_Huni_Maske1,
                                                HM02_Huni_Maske2,
                                                M8_01_Metrik8_1,
                                                M8_02_Metrik8_2,
                                                M8_05_Metrik8_5,
                                                UM_Uzun_Maske,
                                                YM_Yuvarlak_Kisa_Maske,
                                                ST18_02_Spigot_Maske,
                                                ST22_02_Spigot_Maske,
                                                HM03_Huni_Maske3,
                                                HM04_Huni_Maske4,
                                                M8_03_Metrik8_3,
                                                M8_04_Metrik8_4,
                                                M8_06_Metrik8_6,
                                                
                                                ST22_02_Spigot_Zamani,


                                                ST18_01_Spigot_Deger,
                                                ST22_01_Spigot_Deger,
                                                ST18_02_Spigot_Deger,
                                                ST22_02_Spigot_Deger,

                                                HattaGirisTarihi,
                                            
                                                Yuzey_Puruz_Rz30_8_Zamani_boyasiz,
                                                Maskeleme_Zamani,
                                                Maske_Kontrol_Zamani,
                                                Maske_Onayi,
                                                Ham_Red_Kasasi,
                                                Boyasiz_Parca_Rework_Kontrol,
                                                Yuzey_Puruz_Rz50,
                                                Askilama_Kontrol
                                                
                                           


                     




                                            )
                                            VALUES
                                            (
                                                @id,
                                                @Kayit_Tarihi,
                                                @Burossa_Aparat_No_1,
                                                @Barkod,
                                                
                                                @Parca_Durumu,
                                                @Yuzey_Puruz_Rz30_1_boyasiz,
                                                @Yuzey_Puruz_Rz30_2,
                                                @Yuzey_Puruz_Rz30_3,
                                                @Yuzey_Puruz_Rz30_4,
                                                @Yuzey_Puruz_Rz30_5,
                                                @Yuzey_Puruz_Rz30_6,
                                                @Yuzey_Puruz_Rz30_7,
                                                @Yuzey_Puruz_Rz30_8,

                                                @Kaplama_Kalinlik1,
                                                @Kaplama_Kalinlik2,
                                                @Kaplama_Kalinlik3,
                                                @Kaplama_Kalinlik4,

                                                @KM_Kara_Maske,
                                                @CM_Celtikli_Maske,
                                                @ST18_01_Spigot_Maske,
                                                @ST18_01_Spigot_Maske,
                                                @HM01_Huni_Maske1,
                                                @HM02_Huni_Maske2,
                                                @M8_01_Metrik8_1,
                                                @M8_02_Metrik8_2,
                                                @M8_05_Metrik8_5,
                                                @UM_Uzun_Maske,
                                                @YM_Yuvarlak_Kisa_Maske,
                                                @ST18_02_Spigot_Maske,
                                                @ST22_02_Spigot_Maske,
                                                @HM03_Huni_Maske3,
                                                @HM04_Huni_Maske4,
                                                @M8_03_Metrik8_3,
                                                @M8_04_Metrik8_4,
                                                @M8_06_Metrik8_6,
                                                @Maskeleme_Zamani,

                                        
                                          
                                          

                                                @ST18_01_Spigot_Deger,
                                                @ST22_01_Spigot_Deger,
                                                @ST18_02_Spigot_Deger,
                                                @ST22_02_Spigot_Deger,

                                                @HattaGirisTarihi,

                                              
                                                @Yuzey_Puruz_Rz30_8_Zamani_boyasiz,
                                                @Maskeleme_Zamani,
                                                @Maske_Kontrol_Zamani,
                                                @Maske_Onayi,
                                                @Ham_Red_Kasasi,
                                                @Boyasiz_Parca_Rework_Kontrol,
                                                @Yuzey_Puruz_Rz50,
                                                @Askilama_Kontrol

                                               
                                            )";

                                using (SqlCommand cmd = new SqlCommand(query, conn))
                                {
                                    cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
                                    cmd.Parameters.AddWithValue("@Kayit_Tarihi", System.DateTime.UtcNow);

                                    cmd.Parameters.AddWithValue("@Burossa_Aparat_No_1", -1);

                                    cmd.Parameters.AddWithValue("@Barkod", urundegerlerı.Barkod);
                                    cmd.Parameters.AddWithValue("@Parca_Durumu", !urundegerlerı.redDurumu);   // bit
                                    cmd.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_1_boyasiz", (int)(urundegerlerı.boyasızDefectDegerler.defectDeger1));
                                    cmd.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_2", (int)(urundegerlerı.boyasızDefectDegerler.defectDeger2));
                                    cmd.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_3", (int)urundegerlerı.boyasızDefectDegerler.defectDeger3);
                                    cmd.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_4", (int)urundegerlerı.boyasızDefectDegerler.defectDeger4);
                                    cmd.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_5", (int)urundegerlerı.boyasızDefectDegerler.defectDeger5);
                                    cmd.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_6", (int)urundegerlerı.boyasızDefectDegerler.defectDeger6);
                                    cmd.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_7", (int)urundegerlerı.boyasızDefectDegerler.defectDeger7);
                                    cmd.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_8", (int)urundegerlerı.boyasızDefectDegerler.defectDeger8);
                                    cmd.Parameters.AddWithValue("@Askilama_Kontrol", true);

                                    cmd.Parameters.AddWithValue("@Kaplama_Kalinlik1", (int)urundegerlerı.boyasızKalınlıkKontrol.kalınlikDeger1);
                                    cmd.Parameters.AddWithValue("@Kaplama_Kalinlik2", (int)urundegerlerı.boyasızKalınlıkKontrol.kalınlikDeger2);
                                    cmd.Parameters.AddWithValue("@Kaplama_Kalinlik3", (int)urundegerlerı.boyasızKalınlıkKontrol.kalınlikDeger3);
                                    cmd.Parameters.AddWithValue("@Kaplama_Kalinlik4",  (int)urundegerlerı.boyasızKalınlıkKontrol.kalınlikDeger4);


                                    cmd.Parameters.AddWithValue("@KM_Kara_Maske", urundegerlerı.TıpaKontrolVeri.tıpaVarYok1);
                                    cmd.Parameters.AddWithValue("@CM_Celtikli_Maske", urundegerlerı.TıpaKontrolVeri.tıpaVarYok2);
                                    cmd.Parameters.AddWithValue("@ST18_01_Spigot_Maske", urundegerlerı.TıpaKontrolVeri.tıpaVarYok3);
                                    cmd.Parameters.AddWithValue("@ST22_01_Spigot_Maske", urundegerlerı.TıpaKontrolVeri.tıpaVarYok4);
                                    cmd.Parameters.AddWithValue("@HM01_Huni_Maske1", urundegerlerı.TıpaKontrolVeri.tıpaVarYok5);
                                    cmd.Parameters.AddWithValue("@HM02_Huni_Maske2", urundegerlerı.TıpaKontrolVeri.tıpaVarYok6);
                                    cmd.Parameters.AddWithValue("@M8_01_Metrik8_1", urundegerlerı.TıpaKontrolVeri.tıpaVarYok7);
                                    cmd.Parameters.AddWithValue("@M8_02_Metrik8_2", urundegerlerı.TıpaKontrolVeri.tıpaVarYok8);
                                    cmd.Parameters.AddWithValue("@M8_05_Metrik8_5", urundegerlerı.TıpaKontrolVeri.tıpaVarYok9);
                                    cmd.Parameters.AddWithValue("@UM_Uzun_Maske", urundegerlerı.TıpaKontrolVeri.tıpaVarYok10);
                                    cmd.Parameters.AddWithValue("@YM_Yuvarlak_Kisa_Maske", urundegerlerı.TıpaKontrolVeri.tıpaVarYok11);
                                    cmd.Parameters.AddWithValue("@ST18_02_Spigot_Maske", urundegerlerı.TıpaKontrolVeri.tıpaVarYok12);
                                    cmd.Parameters.AddWithValue("@ST22_02_Spigot_Maske", urundegerlerı.TıpaKontrolVeri.tıpaVarYok13);
                                    cmd.Parameters.AddWithValue("@HM03_Huni_Maske3", urundegerlerı.TıpaKontrolVeri.tıpaVarYok13);
                                    cmd.Parameters.AddWithValue("@HM04_Huni_Maske4", urundegerlerı.TıpaKontrolVeri.tıpaVarYok13);
                                    cmd.Parameters.AddWithValue("@M8_03_Metrik8_3", urundegerlerı.TıpaKontrolVeri.tıpaVarYok13);
                                    cmd.Parameters.AddWithValue("@M8_04_Metrik8_4", urundegerlerı.TıpaKontrolVeri.tıpaVarYok13);
                                    cmd.Parameters.AddWithValue("@M8_06_Metrik8_6", urundegerlerı.TıpaKontrolVeri.tıpaVarYok13);

                                    cmd.Parameters.AddWithValue("@Yuzey_Puruz_Rz50", (int)(urundegerlerı.boyasızDefectDegerler.defectDeger1 + urundegerlerı.boyasızDefectDegerler.defectDeger2 + urundegerlerı.boyasızDefectDegerler.defectDeger3 + urundegerlerı.boyasızDefectDegerler.defectDeger4+ urundegerlerı.boyasızDefectDegerler.defectDeger5 + urundegerlerı.boyasızDefectDegerler.defectDeger6 + urundegerlerı.boyasızDefectDegerler.defectDeger7 + urundegerlerı.boyasızDefectDegerler.defectDeger8)/8);

                                    if (urundegerlerı.TıpaKontrolVeri.tıpaVarYok1 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok2 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok3 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok4 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok5 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok6 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok7 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok8 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok9 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok10 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok11 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok12 & urundegerlerı.TıpaKontrolVeri.tıpaVarYok13)
                                    {
                                        cmd.Parameters.AddWithValue("@Maske_Onayi", true);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("@Maske_Onayi", false);
                                    }
                                    cmd.Parameters.AddWithValue("@Ham_Red_Kasasi", urundegerlerı.redDurumu);
                                    cmd.Parameters.AddWithValue("@Boyasiz_Parca_Rework_Kontrol", urundegerlerı.redDurumu);


                                    cmd.Parameters.AddWithValue("@ST18_01_Spigot_Deger", (int)urundegerlerı.TorkDegerler.torkDeger1);
                                    cmd.Parameters.AddWithValue("@ST22_01_Spigot_Deger", (int)urundegerlerı.TorkDegerler.torkDeger2);
                                    cmd.Parameters.AddWithValue("@ST18_02_Spigot_Deger", (int)urundegerlerı.TorkDegerler.torkDeger3);
                                    cmd.Parameters.AddWithValue("@ST22_02_Spigot_Deger", (int)urundegerlerı.TorkDegerler.torkDeger4);

                                    cmd.Parameters.AddWithValue("@HattaGirisTarihi", urundegerlerı.HattaGirisZamani);
                                    
                                    cmd.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_8_Zamani_boyasiz", urundegerlerı.Krt1TaramaBitmeZamani);
                                    cmd.Parameters.AddWithValue("@Maskeleme_Zamani", urundegerlerı.Kvy7OnayGeldiZamani);
                                    cmd.Parameters.AddWithValue("@Maske_Kontrol_Zamani", urundegerlerı.TıpaKontrolZamani);
                                    //cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN5", urundegerlerı.Kvy8AsmaTalebiZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN6", urundegerlerı.Kvy14AskidanUrunİndiZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN7", urundegerlerı.Krt2TaramaBittiZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN8", urundegerlerı.Kvy1618OnayRedZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN9", urundegerlerı.RobotKontrolBitirdiZamani);
                                    cmd.Parameters.AddWithValue("@ISLEM_BITIS_ZAMAN10", urundegerlerı.Kvy9KasalaSinyaliVerdiZamani);





                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }







                    }

                }


            
           

        }
        public class BoyaKalınlık
        {
            public double Kalinlik1 { get; set; }
            public double Kalinlik2 { get; set; }
            public double Kalinlik3 { get; set; }
            public double Kalinlik4 { get; set; }
        }

        public class Datetimelar
        {
            public System.DateTime DateTime1 { get; set; }
            public System.DateTime DateTime2 { get; set; }
            public System.DateTime DateTime3 { get; set; }
            public System.DateTime DateTime4 { get; set; }
            public System.DateTime DateTime5 { get; set; }
            public System.DateTime DateTime6 { get; set; }
        }

        public static BoyaKalınlık liste = new BoyaKalınlık();
        public static void boyaliVeri_Yaz(Plc plc, string connStr)
        {
            try
            {
                plc.Open();

                if (plc.IsConnected)
                {

                    var kvy9UrunVar = umisyDeneme_winform.LıstOfFunctions.PlcBooleanOku(plc, 149, 8290, 7);
                    if (kvy9UrunVar)
                    {

                        Urun urundegerlerı = umisyDeneme_winform.LıstOfFunctions.PlcKVY9Oku(plc);





                        if (!umisyDeneme_winform.LıstOfFunctions.BarkodVarMi(urundegerlerı.Barkod, connStr))
                        {

                            string querykalınlıkal = @"SELECT 
                                Kaplama_Kalinlik1,
                                Kaplama_Kalinlik2,
                                Kaplama_Kalinlik3,
                                Kaplama_Kalinlik4
                            FROM XPODA.dbo.UmisyMakineOtomasyonVeriTablosu 
                            WHERE Barkod = @Barkod";

                            using (SqlConnection conn = new SqlConnection(connStr))
                            {
                                conn.Open();

                                using (SqlCommand cmd = new SqlCommand(querykalınlıkal, conn))
                                {
                                    cmd.Parameters.AddWithValue("@Barkod", urundegerlerı.Barkod);

                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            if (reader.Read())
                                            {
                                                liste.Kalinlik1 = reader.IsDBNull(0) ? 0 : Convert.ToDouble(reader[0]);
                                                liste.Kalinlik1 = reader.IsDBNull(1) ? 0 : Convert.ToDouble(reader[1]);
                                                liste.Kalinlik1 = reader.IsDBNull(2) ? 0 : Convert.ToDouble(reader[2]);
                                                liste.Kalinlik1 = reader.IsDBNull(3) ? 0 : Convert.ToDouble(reader[3]);
                                            }
                                        }
                                    }
                                }
                            }

                            using (SqlConnection conna = new SqlConnection(connStr))
                            {
                                conna.Open();

                                string query = $@"
                                            UPDATE XPODA.dbo.UmisyMakineOtomasyonVeriTablosu
                                                SET 
                                                    Parca_Durumu = @Parca_Durumu,

                                                    Kaplama_Kalinlik1 = @Kaplama_Kalinlik1,
                                                    Kaplama_Kalinlik2 = @Kaplama_Kalinlik2,
                                                    Kaplama_Kalinlik3 = @Kaplama_Kalinlik3,
                                                    Kaplama_Kalinlik4 = @Kaplama_Kalinlik4,

                                                    Yuzey_Puruz_Rz30_1_boyali = @Yuzey_Puruz_Rz30_1_boyali,
                                                    Yuzey_Puruz_Rz30_2_boyali = @Yuzey_Puruz_Rz30_2_boyali,
                                                    Yuzey_Puruz_Rz30_3_boyali = @Yuzey_Puruz_Rz30_3_boyali,
                                                    Yuzey_Puruz_Rz30_4_boyali = @Yuzey_Puruz_Rz30_4_boyali,
                                                    Yuzey_Puruz_Rz30_5_boyali = @Yuzey_Puruz_Rz30_5_boyali,
                                                    Yuzey_Puruz_Rz30_6_boyali = @Yuzey_Puruz_Rz30_6_boyali,
                                                    Yuzey_Puruz_Rz30_7_boyali = @Yuzey_Puruz_Rz30_7_boyali,
                                                    Yuzey_Puruz_Rz30_8_boyali = @Yuzey_Puruz_Rz30_8_boyali,

                                                    Kaplama_Kalinlik5 = @Kaplama_Kalinlik5,
                                                    Kaplama_Kalinlik6 = @Kaplama_Kalinlik6,
                                                    Kaplama_Kalinlik7 = @Kaplama_Kalinlik7,
                                                    Kaplama_Kalinlik8 = @Kaplama_Kalinlik8,
                                                    Kaplama_Kalinlik9 = @Kaplama_Kalinlik9,
                                                    Kaplama_Kalinlik10 = @Kaplama_Kalinlik10,
                                                    Kaplama_Kalinlik11 = @Kaplama_Kalinlik11,
                                                    Kaplama_Kalinlik12 = @Kaplama_Kalinlik12,

                                                    KM_Kara_Maske_Sizma_Bilgi = @KM_Kara_Maske_Sizma_Bilgi,
                                                    ST18_01_Spigot_Sizma_Bilgi = @ST18_01_Spigot_Sizma_Bilgi,
                                                    ST22_01_Spigot_Sizma_Bilgi = @ST22_01_Spigot_Sizma_Bilgi,
                                                    ST18_02_Spigot_Sizma_Bilgi = @ST18_02_Spigot_Sizma_Bilgi,
                                                    ST22_02_Spigot_Sizma_Bilgi = @ST22_02_Spigot_Sizma_Bilgi,

                                                    M8_01_Metrik8_1_Sizma_Bilgi = @M8_01_Metrik8_1_Sizma_Bilgi,
                                                    M8_03_Metrik8_3_Sizma_Bilgi = @M8_03_Metrik8_3_Sizma_Bilgi,
                                                    M8_02_Metrik8_2_Sizma_Bilgi = @M8_02_Metrik8_2_Sizma_Bilgi,
                                                    M8_04_Metrik8_4_Sizma_Bilgi = @M8_04_Metrik8_4_Sizma_Bilgi,
                                                    M8_05_Metrik8_5_Sizma_Bilgi = @M8_05_Metrik8_5_Sizma_Bilgi,
                                                    M8_06_Metrik8_6_Sizma_Bilgi = @M8_06_Metrik8_6_Sizma_Bilgi,
                                                    UM_Uzun_Maske_Sizma_Bilgi = @UM_Uzun_Maske_Sizma_Bilgi,
                                                    YM_Yuvarlak_Kisa_Maske_Sizma_Bilgi = @YM_Yuvarlak_Kisa_Maske_Sizma_Bilgi,
                                                    HM03_Huni_Maske3_Sizma_Bilgi = @HM03_Huni_Maske3_Sizma_Bilgi,
                                                    CM_Celtikli_Maske_Sizma_Bilgi = @CM_Celtikli_Maske_Sizma_Bilgi,
                                                    HM04_Huni_Maske4_Sizma_Bilgi = @HM04_Huni_Maske4_Sizma_Bilgi,
                                                    HM01_Huni_Maske1_Sizma_Bilgi = @HM01_Huni_Maske1_Sizma_Bilgi,
                                                    HM02_Huni_Maske2_Sizma_Bilgi = @HM02_Huni_Maske2_Sizma_Bilgi,
                                                    Ok_kasasi = @Ok_kasasi,    
                                                    Nok_kasasi = @Nok_kasasi, 

                                                
                                                    Kalinlik_4_Zamani = @Kalinlik_4_Zamani,
                                                    Yuzey_Puruz_Rz30_8_Zamani_boyali = @Kalinlik_4_Zamani,
                                                    Rework_1_2_Onay_Zamani = @Rework_1_2_Onay_Zamani,
                                                    Maske_Sensor_Kamera_Zamani = @Maske_Sensor_Kamera_Zamani,
                                                    Kalinlik_12_Zamani = @Kalinlik_12_Zamani,
                                        

                                                    Rework_3_Giris_Tarihi_1 = @Rework_3_Giris_Tarihi_1,
                                                    Rework_3_Giris_Tarihi_2 = @Rework_3_Giris_Tarihi_2,
                                                    Rework_3_Giris_Tarihi_3 = @Rework_3_Giris_Tarihi_3,
                                                    Rework_3_Onay_1 = @Rework_3_Onay_1,
                                                    Rework_3_Onay_2 = @Rework_3_Onay_2,
                                                    Rework_3_Onay_3 = @Rework_3_Onay_3,
                                                   
                                                    Rework_1_2_Onay_Red_verildi = @Rework_1_2_Onay_Red_verildi,
Rework_1_2_Secimi = @Rework_1_2_Secimi
                                                        
                                                        
                                                        
                                                        
                                                       
                                                   

                                                WHERE Barkod =  @Barkod;
                                                                                           ";

                                using (SqlCommand cmda = new SqlCommand(query, conna))
                                {
                                    cmda.Parameters.AddWithValue("@Barkod", urundegerlerı.Barkod);
                                    cmda.Parameters.AddWithValue("@Parca_Durumu", !urundegerlerı.redDurumu);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik1", urundegerlerı.BoyalıKalınlıkKOntrol.kalınlikDeger1-liste.Kalinlik4);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik2", urundegerlerı.BoyalıKalınlıkKOntrol.kalınlikDeger2 - liste.Kalinlik1);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik3", urundegerlerı.BoyalıKalınlıkKOntrol.kalınlikDeger3 - liste.Kalinlik3);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik4", urundegerlerı.BoyalıKalınlıkKOntrol.kalınlikDeger4 - liste.Kalinlik2);

                                    cmda.Parameters.AddWithValue("@Ok_kasasi", !urundegerlerı.redDurumu);
                                    cmda.Parameters.AddWithValue("@Nok_kasasi", urundegerlerı.redDurumu);


                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_1_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger1);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_2_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger2);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_3_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger3);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_4_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger4);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_5_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger5);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_6_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger6);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_7_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger7);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_8_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger8);

                               

                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik5", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger1);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik6", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger2);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik7", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger3);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik8", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger4);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik9", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger5);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik10", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger6);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik11", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger7);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik12", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger8);

                                    cmda.Parameters.AddWithValue("@CM_Celtikli_Maske_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol1);
                                    cmda.Parameters.AddWithValue("@ST18_01_Spigot_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol2);
                                    cmda.Parameters.AddWithValue("@ST22_01_Spigot_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol3);
                                    cmda.Parameters.AddWithValue("@ST18_02_Spigot_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol4);
                                    cmda.Parameters.AddWithValue("@ST22_02_Spigot_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol5);
                                    
                                    cmda.Parameters.AddWithValue("@M8_01_Metrik8_1_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol6);
                                    cmda.Parameters.AddWithValue("@M8_02_Metrik8_2_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol7);
                                    cmda.Parameters.AddWithValue("@M8_05_Metrik8_5_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol8);
                                    cmda.Parameters.AddWithValue("@UM_Uzun_Maske_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol9);
                                    cmda.Parameters.AddWithValue("@YM_Yuvarlak_Kisa_Maske_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol10);
                                    cmda.Parameters.AddWithValue("@HM03_Huni_Maske3_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol11);
                                    cmda.Parameters.AddWithValue("@KM_Kara_Maske_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol12);
                                    cmda.Parameters.AddWithValue("@HM04_Huni_Maske4_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);
                                    cmda.Parameters.AddWithValue("@HM01_Huni_Maske1_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);
                                    cmda.Parameters.AddWithValue("@HM02_Huni_Maske2_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);
                                    cmda.Parameters.AddWithValue("@M8_03_Metrik8_3_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);
                                    cmda.Parameters.AddWithValue("@M8_04_Metrik8_4_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);
                                    cmda.Parameters.AddWithValue("@M8_06_Metrik8_6_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);



                                    




                                    cmda.Parameters.AddWithValue("@Kalinlik_4_Zamani", urundegerlerı.Krt2TaramaBittiZamani);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_8_Zamani_boyali", urundegerlerı.Krt2TaramaBittiZamani);
                                    cmda.Parameters.AddWithValue("@Rework_1_2_Onay_Zamani", urundegerlerı.Kvy1618OnayRedZamani);
                                    cmda.Parameters.AddWithValue("@Maske_Sensor_Kamera_Zamani", urundegerlerı.RobotKontrolBitirdiZamani);
                                    cmda.Parameters.AddWithValue("@Kalinlik_12_Zamani",urundegerlerı.Kvy1618OnayRedZamani);

                                    cmda.Parameters.AddWithValue("@Rework_3_Giris_Tarihi_1", urundegerlerı.krt_kontrol_tarih_1);
                                    cmda.Parameters.AddWithValue("@Rework_3_Giris_Tarihi_2", urundegerlerı.krt_kontrol_tarih_2);
                                    cmda.Parameters.AddWithValue("@Rework_3_Giris_Tarihi_3", urundegerlerı.krt_kontrol_tarih_3);

                                    cmda.Parameters.AddWithValue("@Rework_3_Onay_1", urundegerlerı.Krt_kontrol_onay_1);
                                    cmda.Parameters.AddWithValue("@Rework_3_Onay_2", urundegerlerı.Krt_kontrol_onay_2);
                                    cmda.Parameters.AddWithValue("@Rework_3_Onay_3", urundegerlerı.Krt_kontrol_onay_3);

                                    cmda.Parameters.AddWithValue("@Rework_1_2_Onay_Red_verildi", true );
                                    cmda.Parameters.AddWithValue("@Rework_1_2_Secimi", urundegerlerı.rework_secimi);








                                    cmda.ExecuteNonQuery();
                                }
                            }


                        }
                        else
                        {

                            using (SqlConnection conna = new SqlConnection(connStr))
                            {
                                conna.Open();

                                string query = @"
                                            INSERT INTO XPODA.dbo.UmisyMakineOtomasyonVeriTablosu
                                                    (
                                                        id,HattaGirisTarihi,
                                                        Kayit_Tarihi,
                                                
                                                        Barkod,
                                                        Parca_Durumu,

                                                        Yuzey_Puruz_Rz30_1_boyali,
                                                        Yuzey_Puruz_Rz30_2_boyali,
                                                        Yuzey_Puruz_Rz30_3_boyali,
                                                        Yuzey_Puruz_Rz30_4_boyali,
                                                        Yuzey_Puruz_Rz30_5_boyali,
                                                        Yuzey_Puruz_Rz30_6_boyali,
                                                        Yuzey_Puruz_Rz30_7_boyali,
                                                        Yuzey_Puruz_Rz30_8_boyali,

                                                        Kaplama_Kalinlik5,
                                                        Kaplama_Kalinlik6,
                                                        Kaplama_Kalinlik7,
                                                        Kaplama_Kalinlik8,
                                                        Kaplama_Kalinlik9,
                                                        Kaplama_Kalinlik10,
                                                        Kaplama_Kalinlik11,
                                                        Kaplama_Kalinlik12,

                                                        KM_Kara_Maske_Sizma_Bilgi,
                                                        CM_Celtikli_Maske_Sizma_Bilgi,
                                                        ST18_01_Spigot_Sizma_Bilgi,
                                                        ST22_01_Spigot_Sizma_Bilgi,
                                                        HM01_Huni_Maske1_Sizma_Bilgi,
                                                        HM02_Huni_Maske2_Sizma_Bilgi,
                                                        M8_01_Metrik8_1_Sizma_Bilgi,
                                                        M8_02_Metrik8_2_Sizma_Bilgi,
                                                        M8_05_Metrik8_5_Sizma_Bilgi,
                                                        UM_Uzun_Maske_Sizma_Bilgi,
                                                        YM_Yuvarlak_Kisa_Maske_Sizma_Bilgi,
                                                        ST18_02_Spigot_Sizma_Bilgi,
                                                        ST22_02_Spigot_Sizma_Bilgi,
                                                        HM03_Huni_Maske3_Sizma_Bilgi,   
                                                        HM04_Huni_Maske4_Sizma_Bilgi,
M8_03_Metrik8_3_Sizma_Bilgi,
M8_04_Metrik8_4_Sizma_Bilgi,
M8_06_Metrik8_6_Sizma_Bilgi,
                              Ok_kasasi,       Nok_kasasi,                
                                                        Yuzey_Puruz_Rz30_8_Zamani_boyali,
                                                        Rework_1_2_Onay_Zamani,
                                                        Maske_Sensor_Kamera_Zamani,
Rework_3_Giris_Tarihi_1,
Rework_3_Giris_Tarihi_2,
Rework_3_Giris_Tarihi_3,
Rework_3_Onay_1,
Rework_3_Onay_2,
Rework_3_Onay_3,
Burossa_Aparat_No_1 ,
Rework_1_2_Onay_Red_verildi ,
Rework_1_2_Secimi 
                                                   
                                                    )
                                                    VALUES
                                                    (
@id,@HattaGirisTarihi,
                                                        @Kayit_Tarihi,
                                                  
                                                        @Barkod,
                                                        @Parca_Durumu,

                                                        @Yuzey_Puruz_Rz30_1_boyali,
                                                        @Yuzey_Puruz_Rz30_2_boyali,
                                                        @Yuzey_Puruz_Rz30_3_boyali,
                                                        @Yuzey_Puruz_Rz30_4_boyali,
                                                        @Yuzey_Puruz_Rz30_5_boyali,
                                                        @Yuzey_Puruz_Rz30_6_boyali,
                                                        @Yuzey_Puruz_Rz30_7_boyali,
                                                        @Yuzey_Puruz_Rz30_8_boyali,

                                                        @Kaplama_Kalinlik5,
                                                        @Kaplama_Kalinlik6,
                                                        @Kaplama_Kalinlik7,
                                                        @Kaplama_Kalinlik8,
                                                        @Kaplama_Kalinlik9,
                                                        @Kaplama_Kalinlik10,
                                                        @Kaplama_Kalinlik11,
                                                        @Kaplama_Kalinlik12,

                                                        @KM_Kara_Maske_Sizma_Bilgi,
                                                        @CM_Celtikli_Maske_Sizma_Bilgi,
                                                        @ST18_01_Spigot_Sizma_Bilgi,
                                                        @ST22_01_Spigot_Sizma_Bilgi,
                                                        @HM01_Huni_Maske1_Sizma_Bilgi,
                                                        @HM02_Huni_Maske2_Sizma_Bilgi,
                                                        @M8_01_Metrik8_1_Sizma_Bilgi,
                                                        @M8_02_Metrik8_2_Sizma_Bilgi,
                                                        @M8_05_Metrik8_5_Sizma_Bilgi,
                                                        @UM_Uzun_Maske_Sizma_Bilgi,
                                                        @YM_Yuvarlak_Kisa_Maske_Sizma_Bilgi,
                                                        @ST18_02_Spigot_Sizma_Bilgi,
                                                        @ST22_02_Spigot_Sizma_Bilgi,
                                                        @HM03_Huni_Maske3_Sizma_Bilgi,
                                                        @HM04_Huni_Maske4_Sizma_Bilgi,
                                                        @M8_03_Metrik8_3_Sizma_Bilgi,
                                                        @M8_04_Metrik8_4_Sizma_Bilgi,
                                                        @M8_06_Metrik8_6_Sizma_Bilgi,
@Ok_kasasi,
@Nok_kasasi,

                                                        @Yuzey_Puruz_Rz30_8_Zamani_boyali,
                                                        @Rework_1_2_Onay_Zamani,
                                                        @Maske_Sensor_Kamera_Zamani,
                                        
                                                        @Rework_3_Giris_Tarihi_1,
@Rework_3_Giris_Tarihi_2,
@Rework_3_Giris_Tarihi_3,
@Rework_3_Onay_1,
@Rework_3_Onay_2,
@Rework_3_Onay_3,
@Burossa_Aparat_No_1,
@Rework_1_2_Onay_Red_verildi,
@Rework_1_2_Secimi
                                                    );";

                                using (SqlCommand cmda = new SqlCommand(query, conna))
                                {

                                    cmda.Parameters.AddWithValue("@id", Guid.NewGuid());
                              
                                    cmda.Parameters.AddWithValue("@HattaGirisTarihi", System.DateTime.UtcNow);
                                    cmda.Parameters.AddWithValue("@Kayit_Tarihi", System.DateTime.UtcNow);
                                    

                                    cmda.Parameters.AddWithValue("@Barkod", urundegerlerı.Barkod);
                                    cmda.Parameters.AddWithValue("@Parca_Durumu", !urundegerlerı.redDurumu);
                                    cmda.Parameters.AddWithValue("@Ok_kasasi", !urundegerlerı.redDurumu);
                                    cmda.Parameters.AddWithValue("@Nok_kasasi", urundegerlerı.redDurumu);


                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_1_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger1);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_2_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger2);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_3_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger3);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_4_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger4);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_5_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger5);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_6_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger6);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_7_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger7);
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_8_boyali", (int)urundegerlerı.BoyalıDefectKontrol.defectDeger8);

                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik5", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger1);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik6", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger2);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik7", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger3);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik8", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger4);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik9", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger5);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik10", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger6);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik11", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger7);
                                    cmda.Parameters.AddWithValue("@Kaplama_Kalinlik12", (int)urundegerlerı.manuelKalınlık.manuelkalınlıkDeger8);

                                    cmda.Parameters.AddWithValue("@Burossa_Aparat_No_1", (int)urundegerlerı.Burossa_Aparat_No_1);

                                    cmda.Parameters.AddWithValue("@KM_Kara_Maske_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol1);
                                    cmda.Parameters.AddWithValue("@CM_Celtikli_Maske_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol2);
                                    cmda.Parameters.AddWithValue("@ST18_01_Spigot_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol3);
                                    cmda.Parameters.AddWithValue("@ST22_01_Spigot_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol4);
                                    cmda.Parameters.AddWithValue("@HM01_Huni_Maske1_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol5);

                                    cmda.Parameters.AddWithValue("@HM02_Huni_Maske2_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol6);
                                    cmda.Parameters.AddWithValue("@M8_01_Metrik8_1_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol7);
                                    cmda.Parameters.AddWithValue("@M8_02_Metrik8_2_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol8);
                                    cmda.Parameters.AddWithValue("@M8_05_Metrik8_5_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol9);
                                    cmda.Parameters.AddWithValue("@UM_Uzun_Maske_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol10);
                                    cmda.Parameters.AddWithValue("@YM_Yuvarlak_Kisa_Maske_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol11);
                                    cmda.Parameters.AddWithValue("@ST18_02_Spigot_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol12);
                                    cmda.Parameters.AddWithValue("@ST22_02_Spigot_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);

                                    cmda.Parameters.AddWithValue("@HM03_Huni_Maske3_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);
                                    cmda.Parameters.AddWithValue("@HM04_Huni_Maske4_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);
                                    cmda.Parameters.AddWithValue("@M8_03_Metrik8_3_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);
                                    cmda.Parameters.AddWithValue("@M8_04_Metrik8_4_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);
                                    cmda.Parameters.AddWithValue("@M8_06_Metrik8_6_Sizma_Bilgi", urundegerlerı.KrtRobotKontrol.RobotKontrol13);

                              
                                    cmda.Parameters.AddWithValue("@Yuzey_Puruz_Rz30_8_Zamani_boyali", urundegerlerı.Krt2TaramaBittiZamani);
                                    cmda.Parameters.AddWithValue("@Rework_1_2_Onay_Zamani", urundegerlerı.Kvy1618OnayRedZamani);
                                    cmda.Parameters.AddWithValue("@Maske_Sensor_Kamera_Zamani", urundegerlerı.RobotKontrolBitirdiZamani);

                                    cmda.Parameters.AddWithValue("@Rework_3_Giris_Tarihi_1", urundegerlerı.krt_kontrol_tarih_1);
                                    cmda.Parameters.AddWithValue("@Rework_3_Giris_Tarihi_2", urundegerlerı.krt_kontrol_tarih_2);
                                    cmda.Parameters.AddWithValue("@Rework_3_Giris_Tarihi_3", urundegerlerı.krt_kontrol_tarih_3);

                                    cmda.Parameters.AddWithValue("@Rework_3_Onay_1", urundegerlerı.Krt_kontrol_onay_1);
                                    cmda.Parameters.AddWithValue("@Rework_3_Onay_2", urundegerlerı.Krt_kontrol_onay_2);
                                    cmda.Parameters.AddWithValue("@Rework_3_Onay_3", urundegerlerı.Krt_kontrol_onay_3);

                                    cmda.Parameters.AddWithValue("@Rework_1_2_Onay_Red_verildi", true);
                                    cmda.Parameters.AddWithValue("@Rework_1_2_Secimi", urundegerlerı.rework_secimi);
                                    cmda.ExecuteNonQuery();
                                }
                            }
                        }
                    }









                    }


                }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
              
               // Application.Restart();
            
        }
            finally
            {
                plc.Close();
            }
        }

        


        


        public static Int64 temp_burasso_sarjNo = 0;

        public static void burassoVeri_yaz( string connstr_burosso, string connstr_umisy)
        {
            
                using (SqlConnection conn = new SqlConnection(connstr_burosso))
                {

                    conn.Open();

                    string query = @"SELECT top 1
     
                                      [SarjNo_ChargeNu]
     
                                FROM [BUROSSA_LINKED_SERVER].[BUROSSA_GalvanoDB].[dbo].[Kayitlar_Records]
                                WHERE IstasyonNo_StationNu = 7 
                                ORDER BY TarihSaat_DateTime DESC;";


                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            if (Convert.ToInt64(result) != temp_burasso_sarjNo)
                            {
                                temp_burasso_sarjNo = Convert.ToInt64(result);

                                using (SqlConnection conntoumisy = new SqlConnection(connstr_umisy))
                                {
                                    string querytoumisy = "SELECT [Burossa_Aparat_No_1] FROM [XPODA].[dbo].[UmisyMakineOtomasyonVeriTablosu]";

                                    SqlDataAdapter daaaa = new SqlDataAdapter(querytoumisy, conn);
                                    DataTable dttt = new DataTable();
                                    daaaa.Fill(dttt);

                                    List<int> baraListesi = new List<int>();

                                    foreach (DataRow row in dttt.Rows)
                                    {
                                        baraListesi.Add(Convert.ToInt32(row["Burossa_Aparat_No_1"]));
                                    }

                                    foreach (int bara in baraListesi)
                                    {
                                        if (bara == -1)
                                        {
                                            using (SqlConnection conntoumisy2 = new SqlConnection(connstr_umisy))
                                            {
                                                conntoumisy2.Open();
                                                string updateQuery = "UPDATE [XPODA].[dbo].[UmisyMakineOtomasyonVeriTablosu] SET Burossa_Aparat_No_1 = @SARJNO WHERE Burossa_Aparat_No_1 = @bara;";
                                                
                                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conntoumisy2))
                                                {
                                                    updateCmd.Parameters.AddWithValue("@bara", bara);
                                                    updateCmd.Parameters.AddWithValue("@SARJNO", temp_burasso_sarjNo);
                                                    updateCmd.ExecuteNonQuery();
                                                }
                                            }
                                        }


                                    }
                                }
                            }







                        }
                    }

                }




           
        }
            

        public static void BurassoToPLC(string constr,Plc plc)
        {
            using (SqlConnection conntoumisy2 = new SqlConnection(constr))
            {
                conntoumisy2.Open();

                string query = @"SELECT top 1
     
                                      [SarjNo_ChargeNu]
     
                                FROM [BUROSSA_LINKED_SERVER].[BUROSSA_GalvanoDB].[dbo].[Kayitlar_Records]
                                WHERE IstasyonNo_StationNu = 7 
                                ORDER BY TarihSaat_DateTime DESC;";

                using (SqlCommand cmd = new SqlCommand(query, conntoumisy2))
                {

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        // 🔥 SQL'den gelen değeri short'a çevir
                        short plcDeger = Convert.ToInt16(result);

                        // 🔧 PLC bağlantı
                       
                        plc.Open();

                        if (plc.IsConnected)
                        {
                            plc.Write("DB983.DBW0", plcDeger);
                        }

                        plc.Close();
                    }
                }
            }
        }
    }
}
