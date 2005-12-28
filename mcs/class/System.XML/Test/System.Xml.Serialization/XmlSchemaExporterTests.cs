//
// System.Xml.Serialization.XmlSchemaExporterTests
//
// Author:
//   Gert Driesen (drieseng@users.sourceforge.net)
//
// (C) 2005 Novell
// 

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using NUnit.Framework;

using MonoTests.System.Xml.TestClasses;

namespace MonoTests.System.XmlSerialization
{
	[TestFixture]
	public class XmlSchemaExporterTests
	{
		[Test]
		[Category ("NotWorking")] // on Mono, element is output before type
		public void ExportClass ()
		{
			XmlAttributeOverrides overrides = new XmlAttributeOverrides ();
			XmlAttributes attr = new XmlAttributes ();
			XmlElementAttribute element = new XmlElementAttribute ();
			attr.XmlElements.Add (element);
			overrides.Add (typeof (SimpleClass), "something", attr);

			SimpleClass simple = new SimpleClass ();
			element.ElementName = "saying";
			element.IsNullable = true;
			simple.something = null;

			XmlReflectionImporter ri = new XmlReflectionImporter (overrides, "NS1");

			XmlSchemas schemas = new XmlSchemas ();
			XmlSchemaExporter sx = new XmlSchemaExporter (schemas);

			XmlTypeMapping tm = ri.ImportTypeMapping (typeof (SimpleClass));
			sx.ExportTypeMapping (tm);

			Assert.AreEqual (1, schemas.Count, "#1");

			StringWriter sw = new StringWriter ();
			schemas[0].Write (sw);

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>{0}" +
				"<xs:schema xmlns:tns=\"NS1\" elementFormDefault=\"qualified\" targetNamespace=\"NS1\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">{0}" +
				"  <xs:element name=\"SimpleClass\" nillable=\"true\" type=\"tns:SimpleClass\" />{0}" +
				"  <xs:complexType name=\"SimpleClass\">{0}" +
				"    <xs:sequence>{0}" +
				"      <xs:element minOccurs=\"1\" maxOccurs=\"1\" name=\"saying\" nillable=\"true\" type=\"xs:string\" />{0}" +
				"    </xs:sequence>{0}" +
				"  </xs:complexType>{0}" +
				"</xs:schema>", Environment.NewLine), sw.ToString (), "#2");
		}

		[Test]
		[Category ("NotWorking")] // on Mono, element is output before type
		public void ExportEnum ()
		{
			XmlReflectionImporter ri = new XmlReflectionImporter ("NS2");

			XmlSchemas schemas = new XmlSchemas ();
			XmlSchemaExporter sx = new XmlSchemaExporter (schemas);

			XmlTypeMapping tm = ri.ImportTypeMapping (typeof (EnumDefaultValue));
			sx.ExportTypeMapping (tm);

			Assert.AreEqual (1, schemas.Count, "#1");

			StringWriter sw = new StringWriter ();
			schemas[0].Write (sw);

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>{0}" +
				"<xs:schema xmlns:tns=\"NS2\" elementFormDefault=\"qualified\" targetNamespace=\"NS2\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">{0}" +
				"  <xs:element name=\"EnumDefaultValue\" type=\"tns:EnumDefaultValue\" />{0}" +
				"  <xs:simpleType name=\"EnumDefaultValue\">{0}" +
				"    <xs:list>{0}" +
				"      <xs:simpleType>{0}" +
				"        <xs:restriction base=\"xs:string\">{0}" +
				"          <xs:enumeration value=\"e1\" />{0}" +
				"          <xs:enumeration value=\"e2\" />{0}" +
				"          <xs:enumeration value=\"e3\" />{0}" +
				"        </xs:restriction>{0}" +
				"      </xs:simpleType>{0}" +
				"    </xs:list>{0}" +
				"  </xs:simpleType>{0}" +
				"</xs:schema>", Environment.NewLine), sw.ToString (), "#2");
		}

		[Test]
		[Category ("NotWorking")] // on Mono, element is output before type
		public void ExportXmlSerializable ()
		{
			XmlReflectionImporter ri = new XmlReflectionImporter ("NS3");

			XmlSchemas schemas = new XmlSchemas ();
			XmlSchemaExporter sx = new XmlSchemaExporter (schemas);

			XmlTypeMapping tm = ri.ImportTypeMapping (typeof (Employee));
			sx.ExportTypeMapping (tm);

			Assert.AreEqual (1, schemas.Count, "#1");

			StringWriter sw = new StringWriter ();
			schemas[0].Write (sw);

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>{0}" +
				"<xs:schema xmlns:tns=\"NS3\" elementFormDefault=\"qualified\" targetNamespace=\"NS3\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">{0}" +
				"  <xs:import namespace=\"http://www.w3.org/2001/XMLSchema\" />{0}" +
				"  <xs:element name=\"Employee\" nillable=\"true\">{0}" +
				"    <xs:complexType>{0}" +
				"      <xs:sequence>{0}" +
				"        <xs:element ref=\"xs:schema\" />{0}" +
				"        <xs:any />{0}" +
				"      </xs:sequence>{0}" +
				"    </xs:complexType>{0}" +
				"  </xs:element>{0}" +
				"</xs:schema>", Environment.NewLine), sw.ToString (), "#2");

			ri = new XmlReflectionImporter ("NS4");

			schemas = new XmlSchemas ();
			sx = new XmlSchemaExporter (schemas);

			tm = ri.ImportTypeMapping (typeof (EmployeeSchema));
			sx.ExportTypeMapping (tm);

			Assert.AreEqual (1, schemas.Count, "#3");

			sw = new StringWriter ();
			schemas[0].Write (sw);

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>{0}" +
				"<xs:schema xmlns:tns=\"NS4\" elementFormDefault=\"qualified\" targetNamespace=\"NS4\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">{0}" +
#if NET_2_0
				"  <xs:import namespace=\"urn:types-devx-com\" />{0}" +
				"  <xs:element name=\"employeeRoot\" nillable=\"true\" xmlns:q1=\"urn:types-devx-com\" type=\"q1:employeeRoot\" />{0}" +
#else
				"  <xs:import namespace=\"http://www.w3.org/2001/XMLSchema\" />{0}" +
				"  <xs:element name=\"EmployeeSchema\" nillable=\"true\">{0}" +
				"    <xs:complexType>{0}" +
				"      <xs:sequence>{0}" +
				"        <xs:element ref=\"xs:schema\" />{0}" +
				"        <xs:any />{0}" +
				"      </xs:sequence>{0}" +
				"    </xs:complexType>{0}" +
				"  </xs:element>{0}" +
#endif
				"</xs:schema>", Environment.NewLine), sw.ToString (), "#4");
		}

		[Test]
		public void ExportPrimitive_Int ()
		{
			XmlReflectionImporter ri = new XmlReflectionImporter ("NSPrim1");
			XmlSchemas schemas = new XmlSchemas ();
			XmlSchemaExporter sx = new XmlSchemaExporter (schemas);
			XmlTypeMapping tm = ri.ImportTypeMapping (typeof (int));
			sx.ExportTypeMapping (tm);

			Assert.AreEqual (1, schemas.Count, "#1");

			StringWriter sw = new StringWriter ();
			schemas[0].Write (sw);

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>{0}" +
				"<xs:schema xmlns:tns=\"NSPrim1\" elementFormDefault=\"qualified\" targetNamespace=\"NSPrim1\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">{0}" +
				"  <xs:element name=\"int\" type=\"xs:int\" />{0}" +
				"</xs:schema>", Environment.NewLine), sw.ToString (), "#2");
		}

		[Test]
		[Category ("NotWorking")] // on Mono, only one schema is created
		public void ExportPrimitive_Guid ()
		{
			XmlReflectionImporter ri = new XmlReflectionImporter ("NSPrimGuid");
			XmlSchemas schemas = new XmlSchemas ();
			XmlSchemaExporter sx = new XmlSchemaExporter (schemas);
			XmlTypeMapping tm = ri.ImportTypeMapping (typeof (int));
			sx.ExportTypeMapping (tm);

			Assert.AreEqual (2, schemas.Count, "#1");

			StringWriter sw = new StringWriter ();
			schemas[0].Write (sw);

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>{0}" +
				"<xs:schema xmlns:tns=\"NSPrimGuid\" elementFormDefault=\"qualified\" targetNamespace=\"NSPrimGuid\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">{0}" +
				"  <xs:import namespace=\"http://microsoft.com/wsdl/types/\" />{0}" +
				"  <xs:element name=\"guid\" xmlns:q1=\"http://microsoft.com/wsdl/types/\" type=\"q1:guid\" />{0}" +
				"</xs:schema>", Environment.NewLine), sw.ToString (), "#2");

			sw = new StringWriter ();
			schemas[1].Write (sw);

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>{0}" +
				"<xs:schema xmlns:tns=\"http://microsoft.com/wsdl/types/\" elementFormDefault=\"qualified\" targetNamespace=\"http://microsoft.com/wsdl/types/\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">{0}" +
				"  <xs:simpleType name=\"guid\">{0}" +
				"    <xs:restriction base=\"xs:string\">{0}" +
				"      <xs:pattern value=\"[0-9a-fA-F]{{8}}-[0-9a-fA-F]{{4}}-[0-9a-fA-F]{{4}}-[0-9a-fA-F]{{4}}-[0-9a-fA-F]{{12}}\" />{0}" +
				"    </xs:restriction>{0}" +
				"  </xs:simpleType>{0}" +
				"</xs:schema>", Environment.NewLine), sw.ToString (), "#3");
		}

		public class Employee : IXmlSerializable
		{
			private string _firstName;
			private string _lastName;
			private string _address;

			public XmlSchema GetSchema ()
			{
				return null;
			}

			public void WriteXml (XmlWriter writer)
			{
				writer.WriteStartElement ("employee", "urn:devx-com");
				writer.WriteAttributeString ("firstName", _firstName);
				writer.WriteAttributeString ("lastName", _lastName);
				writer.WriteAttributeString ("address", _address);
				writer.WriteEndElement ();
			}

			public void ReadXml (XmlReader reader)
			{
				XmlNodeType type = reader.MoveToContent ();
				if (type == XmlNodeType.Element && reader.LocalName == "employee") {
					_firstName = reader["firstName"];
					_lastName = reader["lastName"];
					_address = reader["address"];
				}
			}
		}

#if NET_2_0
		[XmlSchemaProvider ("CreateEmployeeSchema")]
#endif
		public class EmployeeSchema : Employee
		{
#if NET_2_0
			public static XmlQualifiedName CreateEmployeeSchema (XmlSchemaSet schemaSet)
			{
				XmlSchema schema = new XmlSchema();
				schema.Id = "EmployeeSchema";
				schema.TargetNamespace = "urn:types-devx-com";
			
				XmlSchemaComplexType type = new XmlSchemaComplexType();
				type.Name = "employeeRoot";
				XmlSchemaAttribute firstNameAttr = new XmlSchemaAttribute();
				firstNameAttr.Name = "firstName";
				type.Attributes.Add(firstNameAttr);

				XmlSchemaAttribute lastNameAttr = new XmlSchemaAttribute();
				lastNameAttr.Name = "lastName";
				type.Attributes.Add(lastNameAttr);

				XmlSchemaAttribute addressAttr = new XmlSchemaAttribute();
				addressAttr.Name = "address";
				type.Attributes.Add(addressAttr);

				XmlSchemaElement employeeElement = new XmlSchemaElement();
				employeeElement.Name = "employee";
				XmlQualifiedName name = new XmlQualifiedName("employeeRoot", "urn:types-devx-com");
				employeeElement.SchemaTypeName = name;

				schema.Items.Add(type);
				schema.Items.Add(employeeElement);
				schemaSet.Add(schema);
				return name;
			}
#endif
		}
	}
}
