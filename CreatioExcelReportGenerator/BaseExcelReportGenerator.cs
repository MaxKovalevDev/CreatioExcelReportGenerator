using System;
using System.Collections.Generic;
using Terrasoft.Core;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Terrasoft.Core.Entities;
using System.IO;

namespace CreatioExcelReportGenerator
{

	/// <summary>
	/// Базовый генератор Excel отчета
	/// </summary>
	public abstract class BaseExcelReportGenerator
	{
		protected UserConnection UserConnection;

		/// <summary>
		/// Дополнительные данные, например, json с данными
		/// </summary>
		protected object AdditionalData;

		/// <summary>
		/// Сущность Excel отчета
		/// </summary>
		protected Entity Report;

		public BaseExcelReportGenerator(UserConnection userConnection, Entity report, object additionalData)
		{
			UserConnection = userConnection;
			AdditionalData = additionalData;
			Report = report;
		}

		public byte[] Run()
		{
			using (MemoryStream stream = new MemoryStream())
			{
				using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
				{
					Generate(document);
					document.Close();
					var result = stream.ToArray();
					return result;
				}
			}
		}

		protected abstract void Generate(SpreadsheetDocument document);

		protected abstract List<(string, UInt32)> PrepareHeaderRow();

		protected virtual Cell CreateCell(string text)
		{
			Cell cell = new Cell();
			cell.StyleIndex = 0U;
			cell.DataType = ResolveCellDataTypeOnValue(text);
			cell.CellValue = new CellValue(text);
			return cell;
		}

		protected virtual Cell CreateCell(string text, uint styleIndex)
		{
			Cell cell = new Cell();
			cell.StyleIndex = styleIndex;
			cell.DataType = ResolveCellDataTypeOnValue(text);
			cell.CellValue = new CellValue(text);
			return cell;
		}

		protected virtual EnumValue<CellValues> ResolveCellDataTypeOnValue(string text)
		{
			int intVal;
			double doubleVal;
			if (int.TryParse(text, out intVal) || double.TryParse(text, out doubleVal))
			{
				return CellValues.Number;
			}
			else
			{
				return CellValues.String;
			}
		}

		protected virtual DocumentFormat.OpenXml.Spreadsheet.Row CreateHeaderRow(List<(string, UInt32)> headers)
		{
			DocumentFormat.OpenXml.Spreadsheet.Row workRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
			foreach (var header in headers)
			{
				workRow.Append(CreateCell(header.Item1, header.Item2));
			}
			return workRow;
		}

		protected virtual Stylesheet GenerateWorkbookStylesPartContent()
		{
			return new Stylesheet(
				new Fonts(
					// 0
					new Font(
						new FontSize() { Val = 10 },
						new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
						new FontName() { Val = "Arial" }),
					// 1
					new Font(
						new Bold(),
						new FontSize() { Val = 8 },
						new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
						new FontName() { Val = "Arial" }),
					//
					new Font(
						new FontSize() { Val = 8 },
						new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
						new FontName() { Val = "Arial" }),
					// 3
					new Font(
						new Bold(),
						new Italic(),
						new FontSize() { Val = 8 },
						new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
						new FontName() { Val = "Arial" }),
					// 4
					new Font(
						new FontSize() { Val = 6 },
						new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
						new FontName() { Val = "Arial" }),
					// 5
					new Font(
						new FontSize() { Val = 6 },
						new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
						new FontName() { Val = "Arial" }),
					// 6
					new Font(
						new FontSize() { Val = 8 },
						new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
						new FontName() { Val = "Arial" }),
					// 7
					new Font(
						new Bold(),
						new FontSize() { Val = 11 },
						new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
						new FontName() { Val = "Arial" }),
					// 8
					new Font(
						new Bold(),
						new FontSize() { Val = 12 },
						new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
						new FontName() { Val = "Arial" })
				),

				new Fills(
					new Fill(                                                           // Стиль под номером 0 - Заполнение ячейки по умолчанию.
						new PatternFill() { PatternType = PatternValues.None }),
					new Fill(                                                           // Стиль под номером 1 - Заполнение ячейки серым цветом
						new PatternFill(
							new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFAAAAAA" } }
							)
						{ PatternType = PatternValues.Solid }),
					new Fill(                                                           // Стиль под номером 2 - Заполнение ячейки красным.
						new PatternFill(
							new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFFFAAAA" } }
						)
						{ PatternType = PatternValues.Solid })
				)
				,
				new Borders(
					// 0
					new Border(                                                         // Стиль под номером 0 - Грани.
						new LeftBorder(),
						new RightBorder(),
						new TopBorder(),
						new BottomBorder(),
						new DiagonalBorder()),
					// 1
					new Border(                                                       // Стиль под номером 1 - Грани
						new LeftBorder(),
						new RightBorder(),
						new TopBorder(),
						new BottomBorder(
							new Color() { Indexed = (UInt32Value)64U }
						)
						{ Style = BorderStyleValues.Thin },
						new DiagonalBorder()),
					// 2
					new Border(                                                         // Стиль под номером 2 - Грани.
						new LeftBorder(
							new Color() { Auto = true }
						)
						{ Style = BorderStyleValues.Thin },
						new RightBorder(
							new Color() { Indexed = (UInt32Value)64U }
						)
						{ Style = BorderStyleValues.Thin },
						new TopBorder(
							new Color() { Auto = true }
						)
						{ Style = BorderStyleValues.Thin },
						new BottomBorder(
							new Color() { Indexed = (UInt32Value)64U }
						)
						{ Style = BorderStyleValues.Thin },
						new DiagonalBorder()),
					// 3
					new Border(                                                         // Стиль под номером 2 - Грани.
						new LeftBorder(
							new Color() { Auto = true }
						)
						{ Style = BorderStyleValues.Thin },
						new RightBorder(
							new Color() { Indexed = (UInt32Value)64U }
						)
						{ Style = BorderStyleValues.Thin },
						new TopBorder(new Color() { Auto = true })
						{ Style = BorderStyleValues.Thin },
						new BottomBorder(
							new Color() { Indexed = (UInt32Value)64U }
						)
						{ Style = BorderStyleValues.Thin },
						new DiagonalBorder()),
					// 4
					new Border(                                                         // Стиль под номером 2 - Грани.
						new LeftBorder(
							new Color() { Auto = true }
						)
						{ Style = BorderStyleValues.Thin },
						new RightBorder(
							new Color() { Indexed = (UInt32Value)64U }
						)
						{ Style = BorderStyleValues.Thin },
						new TopBorder(new Color() { Auto = true })
						{ Style = BorderStyleValues.Thin },
						new BottomBorder(
							new Color() { Indexed = (UInt32Value)64U }
						)
						{ Style = BorderStyleValues.Thin },
						new DiagonalBorder()),
					// 5
					new Border(                                                         // Стиль под номером 2 - Грани.
						new LeftBorder(
							new Color() { Auto = true }
						)
						{ Style = BorderStyleValues.Thin },
						new RightBorder(
							new Color() { Indexed = (UInt32Value)64U }
						)
						{ Style = BorderStyleValues.Thin },
						new TopBorder(new Color() { Auto = true })
						{ Style = BorderStyleValues.Thin },
						new BottomBorder(new Color() { Auto = true })
						{ Style = BorderStyleValues.Thin },
						new DiagonalBorder()),
					// 6
					new Border(                                                         // Стиль под номером 2 - Грани.
						new LeftBorder(
							new Color() { Auto = true }
						)
						{ Style = BorderStyleValues.Thin },
						new RightBorder(
							new Color() { Indexed = (UInt32Value)64U }
						)
						{ Style = BorderStyleValues.Thin },
						new TopBorder(new Color() { Auto = true })
						{ Style = BorderStyleValues.Thin },
						new BottomBorder(new Color() { Auto = true })
						{ Style = BorderStyleValues.Thin },
						new DiagonalBorder())
				),
				new CellFormats(
					// 0
					// Стиль под номером 0 - The default cell style.  (по умолчанию)
					new CellFormat(new Alignment()
					{
						Horizontal = HorizontalAlignmentValues.Left,
						Vertical = VerticalAlignmentValues.Center,
						WrapText = true
					})
					{
						FontId = 0,
						FillId = 0,
						BorderId = 0
					},

					// 1
					// Стиль под номером 1 - Bold
					// Используется в Шапке страницы. Строка "РЕЕСТР"
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Right,
							Vertical = VerticalAlignmentValues.Center,
							WrapText = true
						}
					)
					{
						FontId = 1,
						FillId = 0,
						BorderId = 0,
						ApplyFont = true
					},

					// 2
					// Стиль под номером 2 - Regular
					// Номер реестра ####
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Left,
							Vertical = VerticalAlignmentValues.Center,
							WrapText = true
						}
					)
					{
						FontId = 2,
						FillId = 0,
						BorderId = 0,
						ApplyFont = true
					},

					// 3
					// Шапка таблицы
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Center,
							Vertical = VerticalAlignmentValues.Top,
							WrapText = true
						}
					)
					{
						FontId = 6,
						FillId = 0,
						BorderId = 2,
						ApplyFont = true
					},

					// 4
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Center,
							Vertical = VerticalAlignmentValues.Top,
							WrapText = true
						}
					)
					{
						FontId = 6,
						FillId = 0,
						BorderId = 3,
						ApplyFont = true
					},       // Стиль под номером 3 - Times Roman
							 // 5
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Left,
							Vertical = VerticalAlignmentValues.Center,
							WrapText = true
						}
					)
					{
						FontId = 3,
						FillId = 0,
						BorderId = 0,
						ApplyFill = true
					},       // Стиль под номером 4 - Yellow Fill
							 // 6
					new CellFormat(                                                                   // Стиль под номером 5 - Alignment
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Left,
							Vertical = VerticalAlignmentValues.Top,
							WrapText = true
						}
					)
					{ FontId = 4, FillId = 0, BorderId = 2, ApplyAlignment = true },
					// 7
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Left,
							Vertical = VerticalAlignmentValues.Top,
							WrapText = true
						}
					)
					{
						FontId = 4,
						FillId = 0,
						BorderId = 4,
						ApplyBorder = true
					},      // Стиль под номером 6 - Border
							// 8
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Left,
							Vertical = VerticalAlignmentValues.Top,
							WrapText = true
						}
					)
					{
						FontId = 4,
						FillId = 0,
						BorderId = 5,
						ApplyFont = true
					},       // Стиль под номером 7 - Задает числовой формат полю.
							 // 9
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Left,
							Vertical = VerticalAlignmentValues.Top,
							WrapText = true
						}
					)
					{
						FontId = 5,
						FillId = 0,
						BorderId = 5,
						ApplyFont = true
					},
					// 10
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Left,
							Vertical = VerticalAlignmentValues.Top,
							WrapText = true
						}
					)
					{
						FontId = 4,
						FillId = 0,
						BorderId = 6,
						ApplyBorder = true
					},       // Стиль под номером 7 - Задает числовой формат полю.
							 // 11
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Center,
							Vertical = VerticalAlignmentValues.Top,
							WrapText = true
						}
					)
					{
						FontId = 6,
						FillId = 0,
						BorderId = 5,
						ApplyFont = true
					},
					// 12
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Left,
							Vertical = VerticalAlignmentValues.Top,
							WrapText = true
						}
					)
					{
						FontId = 0,
						FillId = 0,
						BorderId = 2,
						ApplyFont = true
					},
					// 13
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Right,
							Vertical = VerticalAlignmentValues.Top,
							WrapText = true
						}
					)
					{
						FontId = 6,
						FillId = 0,
						BorderId = 0,
						ApplyFont = true
					},
					// 14
					new CellFormat(
						new Alignment()
						{
							Horizontal = HorizontalAlignmentValues.Right,
							Vertical = VerticalAlignmentValues.Top,
							WrapText = true
						}
					)
					{
						FontId = 4,
						FillId = 0,
						BorderId = 2,
						ApplyAlignment = true
					},
					// 15
					new CellFormat(
							new Alignment()
							{
								Horizontal = HorizontalAlignmentValues.Left,
								Vertical = VerticalAlignmentValues.Center,
								WrapText = true
							}
						)
					{
						FontId = 7,
						FillId = 0,
						BorderId = 0,
						ApplyAlignment = true
					},
					// 16
					new CellFormat(
							new Alignment()
							{
								Horizontal = HorizontalAlignmentValues.Left,
								Vertical = VerticalAlignmentValues.Center,
								WrapText = true
							}
						)
					{
						FontId = 0,
						FillId = 0,
						BorderId = 0,
						ApplyAlignment = true
					},
					// 17
					new CellFormat(
							new Alignment()
							{
								Horizontal = HorizontalAlignmentValues.Center,
								Vertical = VerticalAlignmentValues.Center,
								WrapText = true
							}
						)
					{
						FontId = 0,
						FillId = 0,
						BorderId = 0,
						ApplyAlignment = true
					}
						)
			);
		}
	}
}
