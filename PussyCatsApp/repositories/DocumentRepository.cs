using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PussyCatsApp.models;
using System;
using System.Collections.Generic;

namespace PussyCatsApp.repositories
{
    public class DocumentRepository
    {
        private const string connectionString = "Data Source=DESKTOP-SCP6QST;Initial Catalog=PussyCatsDB;Integrated Security=True;TrustServerCertificate=True";
        public List<Document> getDocumentsByUserId(int userId)
        {
            var documents = new List<Document>();

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string query = @"
                SELECT dID         AS Id,
                       userID      AS UserId,
                       nameDocument AS DocumentName,
                       FilePath,
                       UploadDate
                FROM   DOCUMENTS
                WHERE  userID = @UserId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    documents.Add(MapRowToDocument(reader));
                }
            } 
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error retrieving documents for user {userId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred retrieving documents for user {userId}: {ex.Message}"); ;
            }

            return documents;
        }

        public Document getDocumentById(int documentId)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string query = @"
                SELECT dID         AS Id,
                       userID      AS UserId,
                       nameDocument AS DocumentName,
                       FilePath,
                       UploadDate
                FROM   DOCUMENTS
                WHERE  dID = @Id";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", documentId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                    return MapRowToDocument(reader);

            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error retrieving document with ID {documentId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred retrieving document with ID {documentId}: {ex.Message}");
            }
            return null;
        }

        public void addDocument(Document document)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string query = @"
                INSERT INTO DOCUMENTS (userID, nameDocument, FilePath, UploadDate)
                VALUES (@UserId, @DocumentName, @FilePath, @UploadDate)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", document.UserId);
                command.Parameters.AddWithValue("@DocumentName", document.DocumentName);
                command.Parameters.AddWithValue("@FilePath", document.FilePath ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UploadDate", document.UploadDate);

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error adding document for user {document.UserId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred adding document for user {document.UserId}: {ex.Message}");
            }
        }
        public void deleteDocument(int documentId)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string query = "DELETE FROM DOCUMENTS WHERE dID = @Id";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", documentId);
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error deleting document with ID {documentId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred deleting document with ID {documentId}: {ex.Message}");
            }
        }

        // ── Helper: map a data reader row to a Document object ───────────────
        private static Document MapRowToDocument(SqlDataReader reader)
        {
            try
            {
                return new Document
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    UserId = Convert.ToInt32(reader["UserId"]),
                    DocumentName = reader["DocumentName"]?.ToString() ?? string.Empty,
                    FilePath = reader["FilePath"] == DBNull.Value ? null : reader["FilePath"].ToString(),
                    UploadDate = reader["UploadDate"] == DBNull.Value
                                       ? DateTime.MinValue
                                       : Convert.ToDateTime(reader["UploadDate"])
                };
            }
            catch (InvalidCastException castEx)
            {
                Console.WriteLine($"Data mapping error: {castEx.Message}");
                throw new Exception("Error mapping database row to Document object.", castEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected mapping error: {ex.Message}");
                throw;
            }
        }
    }
}