using OpenOrm.SqlProvider.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace OpenOrm.SqlProvider.SqlServer
{
    class SqlServerConnector : BaseConnector, ISqlConnector, IDisposable
    {
        private bool disposed = false;

        public SqlServerConnector()
        {

        }


        #region Parameter
        /// <summary>
        /// Ajoute un paramètre pour l'exécution de la requête (procédure ou sql en dur)
        /// </summary>
        public void AddParameter(string paramName, object value, SqlDbType type)
        {
            SqlParameter param = new SqlParameter();
            if (!paramName.StartsWith("@"))
            {
                paramName = "@" + paramName;
            }
            param.Direction = ParameterDirection.Input;
            param.ParameterName = paramName;
            param.SqlDbType = type;
            param.SqlValue = value;
            param.Size = GetParamSize(value);
            Parameters.Add(param);
        }

        ///// <summary>
        ///// Ajoute un paramètre pour l'exécution de la requête (procédure ou sql en dur)
        ///// </summary>
        //public void AddParameter(string paramName, object value)
        //{
        //    SqlDbType type = SqlServerTools.DetectParameterType(value);
        //    AddParameter(paramName, value, type);
        //}

        public void ClearParameters()
        {
            Parameters = null;
            Parameters = new ArrayList();
        }
        #endregion

        /// <summary>
        /// Exécute la procédure stockée dont le nom est passé en paramètre
        /// Le mot clé internal signifie que la fonction est utilisable par les classes déclarée au sein du fichier
        /// Une valeur de retour @RETURN_VALUE est ajoutée automatiquement aux paramètres
        /// </summary>
        /// <param name="ProcName">Nom de la procédure (Schéma non nécessaire si dbo est utilisé)</param>
        /// <param name="Params">Tableau contenant les paramètres à envoyer à la procédure, et les paramètres de retour</param>
        /// <param name="transac"></param>
        public long ExecuteStoredProcedure(OpenOrmDbConnection cnx, string ProcName)
        {
            long returnValue = 0;
            SqlCommand cmd = new SqlCommand();
            SqlParameter param = default(SqlParameter);
            int i = 0;

            if (cnx.State != ConnectionState.Open)
            {
                cnx.Open();
            }

            cmd.Connection = (SqlConnection)cnx.Connection;
            cmd.CommandText = ProcName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = cnx.Transaction != null ? (SqlTransaction)cnx.Transaction : null;

            //cmd.Parameters.Add(new SqlParameter
            //{
            //    Direction = ParameterDirection.ReturnValue,
            //    SqlDbType = SqlDbType.BigInt,
            //    Scale = 0,
            //    Precision = 10,
            //    ParameterName = "@RETURN_VALUE"
            //});


            if ((Parameters != null))
            {
                for (i = 0; i <= Parameters.Count - 1; i++)
                {
                    param = (SqlParameter)Parameters[i];
                    cmd.Parameters.Add(param);
                }
                //cmd.Prepare();
            }

            try
            {
                cmd.ExecuteNonQuery();
                if ((Parameters != null))
                {
                    for (i = 1; i <= cmd.Parameters.Count - 1; i++)
                    {
                        if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                        {
                            ((SqlParameter)Parameters[i]).Value = cmd.Parameters[i].Value;
                            if (cmd.Parameters[i].ParameterName == "@RETURN_VALUE")
                                returnValue = (long)cmd.Parameters[i].Value;
                        }
                    }
                }

                //ClearParameters();
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible d'executer la commande action : " + ex.Message, ex);
            }
            finally
            {
                cmd.Dispose();
            }

            return returnValue;
        }

        /// <summary>
        /// Exécute la procédure stockée dont le nom est passé en paramètre
        /// Le mot clé internal signifie que la fonction est utilisable par les classes déclarée au sein du fichier
        /// Une valeur de retour @RETURN_VALUE est ajoutée automatiquement aux paramètres
        /// </summary>
        /// <param name="ProcName">Nom de la procédure (Schéma non nécessaire si dbo est utilisé)</param>
        /// <param name="Params">Tableau contenant les paramètres à envoyer à la procédure, et les paramètres de retour</param>
        /// <param name="transac"></param>
        public async Task<long> ExecuteStoredProcedureAsync(OpenOrmDbConnection cnx, string ProcName)
        {
            long returnValue = 0;
            SqlCommand cmd = new SqlCommand();
            SqlParameter param = default(SqlParameter);
            int i = 0;

            if (cnx.State != ConnectionState.Open)
            {
                cnx.Open();
            }

            cmd.Connection = (SqlConnection)cnx.Connection;
            cmd.CommandText = ProcName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = cnx.Transaction != null ? (SqlTransaction)cnx.Transaction : null;

            //cmd.Parameters.Add(new SqlParameter
            //{
            //    Direction = ParameterDirection.ReturnValue,
            //    SqlDbType = SqlDbType.BigInt,
            //    Scale = 0,
            //    Precision = 10,
            //    ParameterName = "@RETURN_VALUE"
            //});


            if ((Parameters != null))
            {
                for (i = 0; i <= Parameters.Count - 1; i++)
                {
                    param = (SqlParameter)Parameters[i];
                    cmd.Parameters.Add(param);
                }
                //cmd.Prepare();
            }

            try
            {
                await cmd.ExecuteNonQueryAsync();
                if ((Parameters != null))
                {
                    for (i = 1; i <= cmd.Parameters.Count - 1; i++)
                    {
                        if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                        {
                            ((SqlParameter)Parameters[i]).Value = cmd.Parameters[i].Value;
                            if (cmd.Parameters[i].ParameterName == "@RETURN_VALUE")
                                returnValue = (long)cmd.Parameters[i].Value;
                        }
                    }
                }

                //ClearParameters();
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible d'executer la commande action : " + ex.Message, ex);
            }
            finally
            {
                cmd.Dispose();
            }

            return returnValue;
        }









        /// <summary>
        /// Exécute la procédure stockée dont le nom est passé en paramètre
        /// Le mot clé internal signifie que la fonction est utilisable par les classes déclarée au sein du fichier
        /// Une valeur de retour @RETURN_VALUE est ajoutée automatiquement aux paramètres
        /// </summary>
        /// <param name="ProcName">Nom de la procédure (Schéma non nécessaire si dbo est utilisé)</param>
        /// <param name="Params">Tableau contenant les paramètres à envoyer à la procédure, et les paramètres de retour</param>
        /// <param name="transac"></param>
        public long ExecuteSql(OpenOrmDbConnection cnx, string Sql)
        {
            long returnValue = 0;
            SqlCommand cmd = new SqlCommand();
            SqlParameter param = default(SqlParameter);
            int i = 0;

            //OpenConnexion();
            if (cnx.State != ConnectionState.Open)
            {
                cnx.Open();
            }

            cmd.Connection = (SqlConnection)cnx.Connection;
            cmd.CommandText = Sql;
            cmd.CommandType = CommandType.Text;
            cmd.Transaction = cnx.Transaction != null ? (SqlTransaction)cnx.Transaction : null;

            //cmd.Parameters.Add(new SqlParameter
            //{
            //    Direction = ParameterDirection.ReturnValue,
            //    SqlDbType = SqlDbType.BigInt,
            //    Scale = 0,
            //    Precision = 10,
            //    ParameterName = "@RETURN_VALUE"
            //});


            if ((Parameters != null))
            {
                for (i = 0; i <= Parameters.Count - 1; i++)
                {
                    param = (SqlParameter)Parameters[i];
                    cmd.Parameters.Add(param);
                }
                //cmd.Prepare();
            }

            try
            {
                cmd.ExecuteNonQuery();
                if ((Parameters != null))
                {
                    for (i = 1; i <= cmd.Parameters.Count - 1; i++)
                    {
                        if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                        {
                            ((SqlParameter)Parameters[i]).Value = cmd.Parameters[i].Value;
                            if (cmd.Parameters[i].ParameterName == "@RETURN_VALUE")
                                returnValue = (long)cmd.Parameters[i].Value;
                        }
                    }
                }

                //ClearParameters();
            }
            catch (Exception ex)
            {

                throw new Exception("Impossible d'executer la commande action : " + ex.Message, ex);
            }
            finally
            {
                cmd.Dispose();
            }

            return returnValue;
        }

        /// <summary>
        /// Exécute la procédure stockée dont le nom est passé en paramètre
        /// Le mot clé internal signifie que la fonction est utilisable par les classes déclarée au sein du fichier
        /// Une valeur de retour @RETURN_VALUE est ajoutée automatiquement aux paramètres
        /// </summary>
        /// <param name="ProcName">Nom de la procédure (Schéma non nécessaire si dbo est utilisé)</param>
        /// <param name="Params">Tableau contenant les paramètres à envoyer à la procédure, et les paramètres de retour</param>
        /// <param name="transac"></param>
        public async Task<long> ExecuteSqlAsync(OpenOrmDbConnection cnx, string Sql)
        {
            long returnValue = 0;
            SqlCommand cmd = new SqlCommand();
            SqlParameter param = default(SqlParameter);
            int i = 0;

            //OpenConnexion();
            if (cnx.State != ConnectionState.Open)
            {
                cnx.Open();
            }

            cmd.Connection = (SqlConnection)cnx.Connection;
            cmd.CommandText = Sql;
            cmd.CommandType = CommandType.Text;
            cmd.Transaction = cnx.Transaction != null ? (SqlTransaction)cnx.Transaction : null;

            //cmd.Parameters.Add(new SqlParameter
            //{
            //    Direction = ParameterDirection.ReturnValue,
            //    SqlDbType = SqlDbType.BigInt,
            //    Scale = 0,
            //    Precision = 10,
            //    ParameterName = "@RETURN_VALUE"
            //});


            if ((Parameters != null))
            {
                for (i = 0; i <= Parameters.Count - 1; i++)
                {
                    param = (SqlParameter)Parameters[i];
                    cmd.Parameters.Add(param);
                }
                //cmd.Prepare();
            }

            try
            {
                await cmd.ExecuteNonQueryAsync();
                if ((Parameters != null))
                {
                    for (i = 1; i <= cmd.Parameters.Count - 1; i++)
                    {
                        if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                        {
                            ((SqlParameter)Parameters[i]).Value = cmd.Parameters[i].Value;
                            if (cmd.Parameters[i].ParameterName == "@RETURN_VALUE")
                                returnValue = (long)cmd.Parameters[i].Value;
                        }
                    }
                }

                //ClearParameters();
            }
            catch (Exception ex)
            {

                throw new Exception("Impossible d'executer la commande action : " + ex.Message, ex);
            }
            finally
            {
                cmd.Dispose();
            }

            return returnValue;
        }









        public DbDataReader GetDataReader(OpenOrmDbConnection cnx, string command, CommandType type)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader dr = default(SqlDataReader);
            SqlParameter param = default(SqlParameter);
            int i = 0;

            try
            {
                //OpenConnexion();
                if (cnx.State != ConnectionState.Open)
                {
                    cnx.Open();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible de se connecter à la base de données : " + ex.Message, ex);
            }

            cmd.Connection = (SqlConnection)cnx.Connection;
            cmd.CommandText = command;
            cmd.CommandType = type;
            cmd.Transaction = cnx.Transaction != null ? (SqlTransaction)cnx.Transaction : null;

            //param = new SqlParameter();
            //param.Direction = ParameterDirection.ReturnValue;
            //param.SqlDbType = SqlDbType.Int;
            //param.Scale = 0;
            //param.Precision = 10;
            //param.ParameterName = "@RETURN_VALUE";
            //cmd.Parameters.Add(param);
            if ((Parameters != null))
            {
                for (i = 0; i <= Parameters.Count - 1; i++)
                {
                    param = (SqlParameter)Parameters[i];
                    cmd.Parameters.Add(param);
                }
                //cmd.Prepare();
            }
            try
            {
                dr = cmd.ExecuteReader();
                if ((Parameters != null))
                {
                    for (i = 1; i <= cmd.Parameters.Count - 1; i++)
                    {
                        if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                        {
                            ((SqlParameter)Parameters[i]).Value = cmd.Parameters[i].Value;
                        }
                    }
                }
                //ClearParameters();
                return dr;
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible de récupérer les données" + Environment.NewLine + ex.Message, ex);
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public async Task<DbDataReader> GetDataReaderAsync(OpenOrmDbConnection cnx, string command, CommandType type)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader dr = default(SqlDataReader);
            SqlParameter param = default(SqlParameter);
            int i = 0;

            try
            {
                //OpenConnexion();
                if (cnx.State != ConnectionState.Open)
                {
                    cnx.Open();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible de se connecter à la base de données : " + ex.Message, ex);
            }

            cmd.Connection = (SqlConnection)cnx.Connection;
            cmd.CommandText = command;
            cmd.CommandType = type;
            cmd.Transaction = cnx.Transaction != null ? (SqlTransaction)cnx.Transaction : null;

            //param = new SqlParameter();
            //param.Direction = ParameterDirection.ReturnValue;
            //param.SqlDbType = SqlDbType.Int;
            //param.Scale = 0;
            //param.Precision = 10;
            //param.ParameterName = "@RETURN_VALUE";
            //cmd.Parameters.Add(param);
            if ((Parameters != null))
            {
                for (i = 0; i <= Parameters.Count - 1; i++)
                {
                    param = (SqlParameter)Parameters[i];
                    cmd.Parameters.Add(param);
                }
                //cmd.Prepare();
            }
            try
            {
                dr = await cmd.ExecuteReaderAsync();
                if ((Parameters != null))
                {
                    for (i = 1; i <= cmd.Parameters.Count - 1; i++)
                    {
                        if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                        {
                            ((SqlParameter)Parameters[i]).Value = cmd.Parameters[i].Value;
                        }
                    }
                }
                //ClearParameters();
                return dr;
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible de récupérer les données" + Environment.NewLine + ex.Message, ex);
            }
            finally
            {
                cmd.Dispose();
            }
        }




        #region IDisposable
        private bool disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés).
                }

                // TODO: libérer les ressources non managées (objets non managés) et remplacer un finaliseur ci-dessous.
                // TODO: définir les champs de grande taille avec la valeur Null.

                if (!disposed)
                {
                    //try
                    //{
                    //    if (_Cnn.State != System.Data.ConnectionState.Closed)
                    //        _Cnn.Close();
                    //}
                    //catch (Exception)
                    //{
                    //    //log
                    //    throw;
                    //}
                    //finally
                    //{
                    //    _Cnn.Dispose();
                    //    disposed = true;
                    //}
                }

                disposedValue = true;
            }
        }

        // TODO: remplacer un finaliseur seulement si la fonction Dispose(bool disposing) ci-dessus a du code pour libérer les ressources non managées.
        // ~SqlConnector() {
        //   // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
        //   Dispose(false);
        // }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
