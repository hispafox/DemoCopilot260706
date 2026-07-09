// Genera el informe diario de tareas en formato Word (.docx)
// Uso: node scripts/generar-informe-diario.js
// Requiere: npm install -g docx  (o npm install docx en la carpeta)

const fs = require("fs");
const path = require("path");

let docxLib;
try {
  docxLib = require("docx");
} catch {
  // Fallback a instalación global
  const globalPath = require("child_process")
    .execSync("npm root -g")
    .toString()
    .trim();
  docxLib = require(path.join(globalPath, "docx"));
}

const {
  Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell,
  HeadingLevel, AlignmentType, BorderStyle, WidthType, ShadingType,
  LevelFormat,
} = docxLib;

// ── Leer tareas desde la API ──────────────────────────────────────────────────
async function obtenerTareas() {
  const res = await fetch("http://localhost:5000/api/tareas");
  if (!res.ok) throw new Error(`Error al llamar a la API: ${res.status}`);
  return await res.json();
}

// ── Helpers ───────────────────────────────────────────────────────────────────
function formatFecha(iso) {
  if (!iso) return "—";
  const d = new Date(iso);
  return d.toLocaleDateString("es-ES", { day: "2-digit", month: "short", year: "numeric" });
}

function badgeColor(prioridad) {
  return { Urgente: "C00000", Alta: "E26B0A", Normal: "375623", Baja: "17375E" }[prioridad] ?? "595959";
}

function colorFila(prioridad) {
  return { Urgente: "FCE4D6", Alta: "FDEBD7", Normal: "EBF1E0", Baja: "DDEEFF" }[prioridad] ?? "FFFFFF";
}

function border() {
  const b = { style: BorderStyle.SINGLE, size: 1, color: "CCCCCC" };
  return { top: b, bottom: b, left: b, right: b };
}

function celda(texto, opts = {}) {
  const { ancho = 2000, negrita = false, color = null, fill = "FFFFFF" } = opts;
  return new TableCell({
    width: { size: ancho, type: WidthType.DXA },
    borders: border(),
    shading: { fill, type: ShadingType.CLEAR },
    margins: { top: 80, bottom: 80, left: 120, right: 120 },
    children: [new Paragraph({
      children: [new TextRun({
        text: texto,
        bold: negrita,
        color: color ?? "000000",
        size: 20,
        font: "Arial",
      })],
    })],
  });
}

function celdaEncabezado(texto, ancho) {
  return new TableCell({
    width: { size: ancho, type: WidthType.DXA },
    borders: border(),
    shading: { fill: "2F5496", type: ShadingType.CLEAR },
    margins: { top: 80, bottom: 80, left: 120, right: 120 },
    children: [new Paragraph({
      children: [new TextRun({ text: texto, bold: true, color: "FFFFFF", size: 20, font: "Arial" })],
    })],
  });
}

// ── Sección por prioridad ─────────────────────────────────────────────────────
function seccionPrioridad(titulo, emoji, tareas) {
  if (tareas.length === 0) return [];

  // Anchuras columnas: Id(700) Título(3800) Vencimiento(1700) Notas(3160)
  const cols = [700, 3800, 1700, 3160];
  const total = cols.reduce((a, b) => a + b, 0); // 9360

  const filaEncabezado = new TableRow({
    tableHeader: true,
    children: [
      celdaEncabezado("Id", cols[0]),
      celdaEncabezado("Título", cols[1]),
      celdaEncabezado("Vencimiento", cols[2]),
      celdaEncabezado("Notas", cols[3]),
    ],
  });

  const filasDatos = tareas.map(t =>
    new TableRow({
      children: [
        celda(String(t.id), { ancho: cols[0], fill: colorFila(t.prioridad) }),
        celda(t.titulo, { ancho: cols[1], negrita: true, fill: colorFila(t.prioridad) }),
        celda(formatFecha(t.fechaVencimiento), { ancho: cols[2], fill: colorFila(t.prioridad) }),
        celda(t.notas ?? "—", { ancho: cols[3], fill: colorFila(t.prioridad) }),
      ],
    })
  );

  return [
    new Paragraph({
      heading: HeadingLevel.HEADING_2,
      spacing: { before: 300, after: 100 },
      children: [new TextRun({ text: `${emoji}  ${titulo} (${tareas.length})`, font: "Arial", color: badgeColor(tareas[0].prioridad) })],
    }),
    new Table({
      width: { size: total, type: WidthType.DXA },
      columnWidths: cols,
      rows: [filaEncabezado, ...filasDatos],
    }),
    new Paragraph({ children: [] }), // espacio
  ];
}

// ── Documento ─────────────────────────────────────────────────────────────────
async function generarDocumento() {
  const todas = await obtenerTareas();
  const pendientes = todas.filter(t => !t.estaCompletada);

  const urgentes = pendientes.filter(t => t.prioridad === "Urgente");
  const altas    = pendientes.filter(t => t.prioridad === "Alta");
  const normales = pendientes.filter(t => t.prioridad === "Normal");
  const bajas    = pendientes.filter(t => t.prioridad === "Baja");

  const hoy = new Date().toLocaleDateString("es-ES", { weekday: "long", day: "numeric", month: "long", year: "numeric" });
  const slug = new Date().toISOString().slice(0, 10);

  const doc = new Document({
    styles: {
      default: { document: { run: { font: "Arial", size: 22 } } },
      paragraphStyles: [
        {
          id: "Heading1", name: "Heading 1", basedOn: "Normal", next: "Normal", quickFormat: true,
          run: { size: 36, bold: true, font: "Arial", color: "1F3864" },
          paragraph: { spacing: { before: 0, after: 200 } },
        },
        {
          id: "Heading2", name: "Heading 2", basedOn: "Normal", next: "Normal", quickFormat: true,
          run: { size: 26, bold: true, font: "Arial" },
          paragraph: { spacing: { before: 200, after: 100 } },
        },
      ],
    },
    sections: [{
      properties: {
        page: {
          size: { width: 11906, height: 16838 }, // A4
          margin: { top: 1440, right: 1080, bottom: 1440, left: 1080 },
        },
      },
      children: [
        // Título
        new Paragraph({
          heading: HeadingLevel.HEADING_1,
          children: [new TextRun({ text: "Informe Diario de Tareas", font: "Arial", bold: true, color: "1F3864" })],
        }),
        new Paragraph({
          children: [new TextRun({ text: hoy.charAt(0).toUpperCase() + hoy.slice(1), font: "Arial", size: 22, color: "595959" })],
          spacing: { after: 200 },
        }),

        // Resumen numérico
        new Paragraph({
          heading: HeadingLevel.HEADING_2,
          children: [new TextRun({ text: "Resumen", font: "Arial" })],
        }),
        new Table({
          width: { size: 9360, type: WidthType.DXA },
          columnWidths: [2340, 2340, 2340, 2340],
          rows: [
            new TableRow({ tableHeader: true, children: [
              celdaEncabezado("Urgente", 2340), celdaEncabezado("Alta", 2340),
              celdaEncabezado("Normal", 2340), celdaEncabezado("Baja", 2340),
            ]}),
            new TableRow({ children: [
              celda(String(urgentes.length), { ancho: 2340, negrita: true, fill: "FCE4D6" }),
              celda(String(altas.length),    { ancho: 2340, negrita: true, fill: "FDEBD7" }),
              celda(String(normales.length), { ancho: 2340, negrita: true, fill: "EBF1E0" }),
              celda(String(bajas.length),    { ancho: 2340, negrita: true, fill: "DDEEFF" }),
            ]}),
          ],
        }),
        new Paragraph({ children: [] }),

        // Secciones por prioridad
        ...seccionPrioridad("Urgente", "🔴", urgentes),
        ...seccionPrioridad("Alta",    "🟠", altas),
        ...seccionPrioridad("Normal",  "🟡", normales),
        ...seccionPrioridad("Baja",    "🟢", bajas),

        // Pie
        new Paragraph({
          alignment: AlignmentType.RIGHT,
          spacing: { before: 400 },
          children: [new TextRun({ text: `Generado automáticamente — ${new Date().toLocaleTimeString("es-ES")}`, size: 18, color: "AAAAAA", font: "Arial" })],
        }),
      ],
    }],
  });

  const buffer = await Packer.toBuffer(doc);
  const ruta = path.join(__dirname, `../docs/informe-diario-${slug}.docx`);
  fs.writeFileSync(ruta, buffer);
  console.log(`✅ Informe generado: ${ruta}`);
}

generarDocumento().catch(err => { console.error("❌", err.message); process.exit(1); });
