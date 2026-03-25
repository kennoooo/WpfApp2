using System.Data.SqlClient;

namespace WpfApp2
{
    public class PartDescription
    {
        public string Text { get; set; }

        public static string GetSql(int typeId)
        {
            string baseSelect = "SELECT b.*, m.name as mName ";
            string joins = "FROM basepart$ b JOIN manufacturer$ m ON b.manufacturerid = m.id ";

            switch (typeId)
            {
                case 1:
                    return baseSelect + ", s.name as sName, c.numberofcores as cores " +
                    joins + "JOIN cpu$ c ON b.id = c.id JOIN socket$ s ON c.socketid = s.id";
                case 2:
                    return baseSelect + ", g.videomemory as vram, g.chipfrequency as freq " +
                    joins + "JOIN gpu$ g ON b.id = g.id";
                case 3:
                    return baseSelect + ", r.capacity as cap, mt.name as memName " +
                    joins + "JOIN ram$ r ON b.id = r.id JOIN memorytype$ mt ON r.memorytypeid = mt.id";
                case 4:
                    return baseSelect + ", s.name as sName, f.name as fName " +
                    joins + "JOIN motherboard$ mb ON b.id = mb.id JOIN socket$ s ON mb.socketid = s.id JOIN formfactor$ f ON mb.formfactorid = f.id";
                case 6:
                    return baseSelect + ", p.power as pwr, cert.name as cName " +
                    joins + "JOIN powersupply$ p ON b.id = p.id JOIN certificate$ cert ON p.certificationid = cert.id";
                default:
                    return baseSelect + joins;
            }
        }


        public static PartDescription Create(int typeId, SqlDataReader reader)
        {
            string info = "Характеристики отсутствуют";
            try
            {
                if (typeId == 1) info = $"Сокет: {reader["sName"]}, Ядер: {reader["cores"]}";
                else if (typeId == 2) info = $"Память: {reader["vram"]} ГБ, Частота: {reader["freq"]} МГц";
                else if (typeId == 3) info = $"Тип: {reader["memName"]}, Объем: {reader["cap"]} ГБ";
                else if (typeId == 4) info = $"Сокет: {reader["sName"]}, Формат: {reader["fName"]}";
                else if (typeId == 6) info = $"Мощность: {reader["pwr"]} Вт, Сертификат: {reader["cName"]}";
            }
            catch { }

            return new PartDescription { Text = info };
        }
    }
}