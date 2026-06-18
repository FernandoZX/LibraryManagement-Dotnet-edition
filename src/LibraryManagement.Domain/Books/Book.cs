using LibraryManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Books
{
    public class Book : Entity
    {
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Genre { get; private set; }
        public string Isbn { get; private set; }
        public int TotalCopies { get; private set; }
        public int AvailableCopies { get; private set; }

        private Book() { } // EF Core

        public Book(string title, string author, string genre, string isbn, int totalCopies)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required.");
            if (string.IsNullOrWhiteSpace(author)) throw new DomainException("Author is required.");
            if (string.IsNullOrWhiteSpace(isbn)) throw new DomainException("ISBN is required.");
            if (totalCopies < 0) throw new DomainException("Total copies cannot be negative.");

            Title = title.Trim();
            Author = author.Trim();
            Genre = genre?.Trim() ?? string.Empty;
            Isbn = isbn.Trim();
            TotalCopies = totalCopies;
            AvailableCopies = totalCopies; // al crear, todas disponibles
        }

        public bool IsAvailable => AvailableCopies > 0;

        public void Borrow()
        {
            if (!IsAvailable)
                throw new DomainException("No copies available to borrow.");
            AvailableCopies--;
        }

        public void Return()
        {
            if (AvailableCopies >= TotalCopies)
                throw new DomainException("All copies are already in the library.");
            AvailableCopies++;
        }

        public void UpdateDetails(string title, string author, string genre, string isbn, int totalCopies)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required.");

            var borrowed = TotalCopies - AvailableCopies;
            if (totalCopies < borrowed)
                throw new DomainException("Total copies cannot be less than the copies currently borrowed.");

            Title = title.Trim();
            Author = author.Trim();
            Genre = genre?.Trim() ?? string.Empty;
            Isbn = isbn.Trim();
            TotalCopies = totalCopies;
            AvailableCopies = totalCopies - borrowed; // mantiene la consistencia
        }

    }
}
