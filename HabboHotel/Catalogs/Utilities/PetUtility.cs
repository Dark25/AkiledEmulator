using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Pets;

namespace Akiled.HabboHotel.Catalog.Utilities
{
    public static class PetUtility
    {
        public static bool CheckPetName(string PetName)
        {
            if (PetName.Length < 1 || PetName.Length > 16)
                return false;

            if (!AkiledEnvironment.IsValidAlphaNumeric(PetName))
                return false;

            return true;
        }

        public static Pet CreatePet(int UserId, string Name, int Type, string Race, string Color)
        {
            Pet pet = new Pet(404, UserId, 0, Name, Type, Race, Color, 0, 100, 100, 0, (double)AkiledEnvironment.GetUnixTimestamp(), 0, 0, 0.0, 0, 1, -1, false);
            pet.DBState = DatabaseUpdateState.NeedsUpdate;

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO user_pets (user_id,name,type,race,color,expirience,energy,createstamp) VALUES (" + pet.OwnerId + ",@" + pet.PetId + "name," + pet.Type + ",@" + pet.PetId + "race,@" + pet.PetId + "color,0,100,'" + pet.CreationStamp + "')");
                dbClient.AddParameter(pet.PetId + "name", pet.Name);
                dbClient.AddParameter(pet.PetId + "race", pet.Race);
                dbClient.AddParameter(pet.PetId + "color", pet.Color);
                pet.PetId = (int)dbClient.InsertQuery();
            }
            return pet;
        }
    }
}
