using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using StudentPortal.Data.Entites;
using StudentPortal.Data;

[TestClass]
public class StudentsControllerTests
{
    private ApplicationDbContext _context;
    private StudentsController _controller;

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Assurez-vous d'utiliser un nom de base de données unique pour chaque test
            .Options;

        _context = new ApplicationDbContext(options);

        // Seed the database with test data
        _context.Students.Add(new Student { Id = Guid.NewGuid(), Name = "Test1", Email="test@pfe.com" , Phone="06999999" , Subscribed=true });
        _context.Students.Add(new Student { Id = Guid.NewGuid(), Name = "Test2", Email = "test2@pfe.com", Phone = "06999999", Subscribed = true });
        _context.SaveChanges();

        _controller = new StudentsController(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public async Task Index_ReturnsAViewResult_WithAListOfStudents()
    {
        // Act
        var result = await _controller.Index();

        // Assert
        Assert.IsInstanceOfType(result, typeof(ViewResult), "Le résultat n'est pas du type ViewResult.");
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult, "Le ViewResult est null.");

        var model = viewResult.Model as List<Student>;
        Assert.IsNotNull(model, "Le modèle n'est pas du type List<Student>.");
        Assert.AreEqual(2, model.Count, "Le nombre d'étudiants attendu n'est pas égal à 2.");
    }


    // Ajoutez d'autres méthodes de test ici...

    [TestMethod]
    public async Task Create_Post_ValidModel_ReturnsRedirectToIndex()
    {
        // Arrange
        var student = new Student { Name = "Nouveau Étudiant", Email = "nouveau@example.com", Phone="078888888888" , Subscribed=false };

        // Act
        var result = await _controller.Create(student);

        // Assert
        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "Le résultat n'est pas du type RedirectToActionResult.");
        var redirectToActionResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectToActionResult, "Le RedirectToActionResult est null.");
        Assert.AreEqual("Index", redirectToActionResult.ActionName, "L'action de redirection n'est pas Index.");
    }
    [TestMethod]
    public async Task Edit_Post_ValidModel_ReturnsRedirectToIndex()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var originalStudent = new Student { Id = studentId, Name = "Étudiant Existante", Email = "existante@example.com", Phone = "0999999999", Subscribed = true };
        _context.Students.Add(originalStudent);
        await _context.SaveChangesAsync();

        var updatedStudent = new Student { Id = studentId, Name = "Étudiant Modifié", Email = "modifie@example.com", Phone = "0999999999", Subscribed = false };

        // Detach the original student to avoid tracking issues
        _context.Entry(originalStudent).State = EntityState.Detached;

        // Act
        var result = await _controller.Edit(studentId, updatedStudent);

        // Assert
        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "Le résultat n'est pas du type RedirectToActionResult.");
        var redirectToActionResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectToActionResult, "Le RedirectToActionResult est null.");
        Assert.AreEqual("Index", redirectToActionResult.ActionName, "L'action de redirection n'est pas Index.");
    }


    [TestMethod]
    public async Task DeleteConfirmed_Post_ValidId_ReturnsRedirectToIndex()
    {
        // Arrange
        var student = new Student { Id = Guid.NewGuid(), Name = "Étudiant à Supprimer", Email = "supprimer@example.com", Phone = "0777777777", Subscribed = true };
        _context.Students.Add(student);
        _context.SaveChanges();

        // Act
        var result = await _controller.DeleteConfirmed(student.Id);

        // Assert
        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "Le résultat n'est pas du type RedirectToActionResult.");
        var redirectToActionResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectToActionResult, "Le RedirectToActionResult est null.");
        Assert.AreEqual("Index", redirectToActionResult.ActionName, "L'action de redirection n'est pas Index.");
    }

}
