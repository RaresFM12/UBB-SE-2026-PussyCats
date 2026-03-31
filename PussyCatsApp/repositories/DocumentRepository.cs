using Microsoft.Data.SqlClient;
using PussyCatsApp.models;
using System;
using System.Collections.Generic;

namespace PussyCatsApp.repositories
{
    public class DocumentRepository
    {
        private const string connectionString = "Data Source=DESKTOP-C5LH746\\SQLEXPRESS;Initial Catalog=PussyCatsDB;Integrated Security=True;Trust Server Certificate=True";

        // ── Retrieve all documents for a given user ──────────────────────────
        public List<Document> getByUserId(int userId)
        {
            var documents = new List<Document>();

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

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                documents.Add(MapRow(reader));
            }

            return documents;
        }

        // ── Retrieve a single document by its primary key ────────────────────
        public Document getById(int id)
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

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return MapRow(reader);

            return null;
        }

        // ── Insert a new document record ─────────────────────────────────────
        public void add(Document doc)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            const string query = @"
                INSERT INTO DOCUMENTS (userID, nameDocument, FilePath, UploadDate)
                VALUES (@UserId, @DocumentName, @FilePath, @UploadDate)";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", doc.UserId);
            cmd.Parameters.AddWithValue("@DocumentName", doc.DocumentName);
            cmd.Parameters.AddWithValue("@FilePath", doc.FilePath ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UploadDate", doc.UploadDate);

            cmd.ExecuteNonQuery();
        }

        // ── Remove a document record by primary key ──────────────────────────
        public void delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            const string query = "DELETE FROM DOCUMENTS WHERE dID = @Id";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        // ── Helper: map a data reader row to a Document object ───────────────
        private static Document MapRow(SqlDataReader reader)
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
    }
}