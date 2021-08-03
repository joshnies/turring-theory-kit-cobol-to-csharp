// Author: Joshua Nies
//
// Copyright © Turring 2021. All Rights Reserved.
//
using System;
using TheoryKitCOBOL;

namespace Example
{
    public class ExampleApplication
    {
        private COBOLFile phonebook;
        private COBOLVar phoneAreaCode;
        private COBOLVar phoneFirst;
        private COBOLVar phoneSecond;
        private COBOLVar phoneAreaIsLocal;
        private COBOLGroup phoneNumber;

        #region Ctlno
        private COBOLVar CtlnDateStamp;
        private COBOLVar CtlnTimeStamp;
        private COBOLVar CtlnPgmid;
        private COBOLGroup CtlnDateTimePgmid;

        private COBOLVar CtlnSysctlno;
        private COBOLGroup CtlnCtlnoKey;

        private COBOLVar CtlnSysctlnoType;
        private COBOLGroup CtlnCtlnoData;

        private COBOLVar Filler111;

        private COBOLGroup CtlnCtlnoRecord;
        private COBOLGroup Ctlno;
        #endregion

        private DatabaseConnection db;

        public void Run()
        {
            // Initialize COBOL vars and groups
            phonebook = new COBOLFile(@"test.txt");
            phoneAreaCode = new COBOLVar(0, size: 3);
            phoneFirst = new COBOLVar(0, size: 3);
            phoneSecond = new COBOLVar(0, size: 4);
            phoneAreaIsLocal = new COBOLVar(
                false,
                size: 1,
                conditionVar: phoneAreaCode,
                conditionFunc: item => item == 619
            );

            phoneNumber = new COBOLGroup(
                phoneAreaCode,
                phoneFirst,
                phoneSecond,
                phoneAreaIsLocal
            );

            #region Ctlno
            CtlnDateStamp = new COBOLVar(new string(' ', 06), size: 06);
            CtlnTimeStamp = new COBOLVar(new string(' ', 06), size: 06);
            CtlnPgmid = new COBOLVar(new string(' ', 04), size: 04);
            CtlnDateTimePgmid = new COBOLGroup(
                CtlnDateStamp,
                CtlnTimeStamp,
                CtlnPgmid
            );

            CtlnSysctlno = new COBOLVar(new string(' ', 10), size: 10);
            CtlnCtlnoKey = new COBOLGroup(CtlnSysctlno);

            CtlnSysctlnoType = new COBOLVar(new string(' ', 4), size: 4);
            Filler111 = new COBOLVar(new string(' ', 0018), size: 0018);
            CtlnCtlnoData = new COBOLGroup(CtlnSysctlnoType, Filler111);

            CtlnCtlnoRecord = new COBOLGroup(
                CtlnDateTimePgmid,
                CtlnCtlnoKey,
                CtlnCtlnoData
            );
            Ctlno = new COBOLGroup(CtlnCtlnoRecord);
            #endregion

            // Initialize database
            db = new DatabaseConnection(
                host: Environment.GetEnvironmentVariable("DB_HOST"),
                user: Environment.GetEnvironmentVariable("DB_USER"),
                password: Environment.GetEnvironmentVariable("DB_PASS"),
                databaseName: Environment.GetEnvironmentVariable("DB_NAME")
            );

            phonebook.AttachData(phoneNumber);

            // Get user input
            Console.Write("Phone: ");
            phoneNumber.Set(Console.ReadLine());

            // Output to console
            Console.WriteLine($"You entered: {phoneNumber}");
            Console.WriteLine($"Area code: {phoneAreaCode}");
            Console.WriteLine($"First: {phoneFirst}");
            Console.WriteLine($"Second: {phoneSecond}");
            Console.WriteLine($"Is local?: {phoneAreaIsLocal.value == true}");

            // Output to file (implicit write)
            //phonebook.Append();

            // Get data from database
            var query = SQLQueryBuilder.Select().From("ctlno").Limit(1);
            Ctlno.Set(db.Query(query));
            Console.WriteLine($"CTLNO from database: \"{Ctlno}\"");
        }
    }

    public class EntryPoint
    {
        static void Main()
        {
            var app = new ExampleApplication();
            app.Run();
        }
    }
}
