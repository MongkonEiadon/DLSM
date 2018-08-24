using Novell.Directory.Ldap;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;


namespace TestProgram
{
    class Program
    {
        public void userList()
        {
            string LDAP_server = "localhost";
            int ldapPort = 1920;
            string loginON = "uid=admin,ou=system";
            string password = "secret";
            string searchBase = "ou=user,o=Company";
            string searchFilter = "ObjectClass=UserLogin,Password,Name,TelNo,Email";

            try
            {
                LdapConnection conn = new LdapConnection();
                Console.WriteLine("Connecting to " + LDAP_server);
                conn.Connect(LDAP_server, ldapPort);
                conn.Bind(loginON, password);
                string[] requiredAttri = { "cn", "sn", "uid" };
                LdapSearchResults result = conn.Search(searchBase,
                                            LdapConnection.SCOPE_SUB,
                                            searchFilter,
                                            requiredAttri,
                                            false);
                while (result.hasMore())
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        nextEntry = result.next();
                    }
                    catch (LdapException ex)
                    {
                        Console.WriteLine("LDAPconnection error:" + ex.LdapErrorMessage);
                        continue;
                    }
                    Console.WriteLine("\n" + nextEntry.DN);
                    LdapAttributeSet ldapattri = nextEntry.getAttributeSet();
                    System.Collections.IEnumerator ienum = ldapattri.GetEnumerator();
                    while (ienum.MoveNext())
                    {
                        LdapAttribute attri = (LdapAttribute)ienum.Current;
                        string attriName = attri.Name;
                        string attriVal = attri.StringValue;
                        Console.WriteLine("\t" + attriName + "\tValue = \t" + attriVal);
                    }
                }
                conn.Disconnect();
            }
            catch(LdapException ex)
            {
                Console.WriteLine("LDAPconnection error:" + ex.LdapErrorMessage);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("LDAPconnection error:" + ex.Message);
                return;
            }

        }

        static void Main(string[] args)
        {
            Program programeObj = new Program();
            programeObj.userList();
        }


        //public static int SearchDetailById(string domain, string uid, string pass, string filter)
        //{
        //    try
        //    {
        //        using (DirectoryEntry entry = new DirectoryEntry(domain, uid, pass, AuthenticationTypes.None))
        //        {
        //            using (DirectorySearcher ds = new DirectorySearcher(entry, String.Format("(uid={0})", filter)))
        //            {
        //                SearchResult results;
        //                //ds.Filter = string.Format("uid={0}", filter);

        //                TimeSpan LdapTimeout = new TimeSpan(0, 0, 3600);
        //                try
        //                {
        //                    ds.ServerTimeLimit = LdapTimeout;
        //                    ds.ClientTimeout = LdapTimeout;
        //                    PropertiesToLoad(ds);
        //                    results = ds.FindOne();

        //                    //keep properties
        //                    json = Invokes(results);

        //                }
        //                catch (Exception e)
        //                {
        //                    //LdapPwIncorrect
        //                    status = new StatusModel() { Code = ConstStatus.LdapPwIncorrect };
        //                    return 0;
        //                }

        //                //LdapNotFoundUser
        //                if (results.Properties.PropertyNames == null || results.Properties.PropertyNames.Count == 0)
        //                {
        //                    //LdapUserNotFound
        //                    status = new StatusModel() { Code = ConstStatus.LdapUserNotFound };
        //                    return 0;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //LdapError
        //        status = new StatusModel() { Code = ConstStatus.LdapError, Desc = ex.Message };
        //        return -1;
        //    }
        //    return 1;
        //}
    }
}