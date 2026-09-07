// Re-runnable conversion of the portfolio's oversized media into modern web formats.
//
//   cd tools/media && npm install && npm run convert
//
// Animated GIFs that only ever appear as small card thumbnails collapse to a single WebP
// still; the ones whose motion is the actual content become a muted looping MP4 + WebM
// pair with a WebP poster. Every raster image is re-encoded to WebP at the size it is
// actually displayed at. Nothing here ships to the browser - these are dev-time tools.

import { spawn } from 'node:child_process';
import { existsSync, mkdirSync, readFileSync, statSync, unlinkSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

import ffmpegPath from 'ffmpeg-static';
import ffprobeStatic from 'ffprobe-static';
import sharp from 'sharp';

const here = path.dirname(fileURLToPath(import.meta.url));
const wwwroot = path.resolve(here, '..', '..', 'src', 'PersonalPortfolio.Blazor', 'wwwroot');
const gifDir = path.join(wwwroot, 'gifs');
const imageDir = path.join(wwwroot, 'images');
const videoDir = path.join(wwwroot, 'video');
const tempDir = path.join(here, '.tmp');

const ffprobePath = ffprobeStatic.path;

// GIFs that are only ever rendered as a 180px card thumbnail. Issue #2 asks for these to
// become plain images, and at that size the animation is noise rather than information.
const thumbnailGifs = [
    'object-detection-python_background.gif',
    'pgt-editor_background.gif',
    'pgm_background.gif',
    'flocking_background.gif',
    'IMGui-example_background.gif',
    'snake-game-java_background.gif'
];

// GIFs embedded in project write-ups, where the motion is the point.
const clipGifs = [
    'SR2S_World.gif',
    'spacecraft_quests.gif',
    'spacecraft_interaction.gif',
    'traversial_gameplay.gif',
    'spacecraft_multiplayer.gif',
    'interaction-system.gif',
    'sound-ai.gif',
    'traversial_enemy_ai_alt_02.gif',
    'SR2S_Car_AI_01.gif',
    'SR2S_Player_Controller_01.gif',
    'SR2S_Cardboard_01.gif',
    'SR2S_Ped_AI_02.gif',
    'spacecraft_world_generation.gif'
];

// Nothing references this one; it exists only to be deleted.
const unusedImages = ['otherImage_01.png'];

// Rendered at 220px inside a MudAvatar, so 440px covers 2x displays.
const imageMaxWidths = { 'headshot_01.png': 440 };
const defaultImageMaxWidth = 1280;

const report = [];

function run(bin, args) {
    return new Promise((resolve, reject) => {
        const child = spawn(bin, args, { stdio: ['ignore', 'pipe', 'pipe'] });
        let stdout = '';
        let stderr = '';
        child.stdout.on('data', (d) => (stdout += d));
        child.stderr.on('data', (d) => (stderr += d));
        child.on('error', reject);
        child.on('close', (code) =>
            code === 0
                ? resolve(stdout)
                : reject(new Error(`${path.basename(bin)} exited ${code}\n${stderr.slice(-2000)}`))
        );
    });
}

function mb(bytes) {
    return bytes / (1024 * 1024);
}

function sizeOf(file) {
    return existsSync(file) ? statSync(file).size : 0;
}

async function hasEncoder(name) {
    const encoders = await run(ffmpegPath, ['-hide_banner', '-encoders']);
    return encoders.includes(` ${name} `);
}

async function probe(file) {
    const raw = await run(ffprobePath, [
        '-v', 'error',
        '-select_streams', 'v:0',
        '-show_entries', 'stream=width,height,avg_frame_rate:format=duration',
        '-of', 'json',
        file
    ]);
    const parsed = JSON.parse(raw);
    const stream = parsed.streams?.[0] ?? {};
    const [num, den] = String(stream.avg_frame_rate ?? '10/1').split('/').map(Number);
    const fps = den > 0 && num > 0 ? num / den : 10;
    return {
        width: stream.width ?? 0,
        height: stream.height ?? 0,
        fps,
        duration: Number(parsed.format?.duration ?? 0)
    };
}

// Clips are rendered inside a MaxWidth.Medium container, so anything past 960px is wasted
// bytes. trunc()*2 forces an even width, which libx264's yuv420p pixel format requires.
const clipMaxWidth = 960;

function scaleFilter(fps) {
    return `fps=${fps},scale='2*trunc(min(${clipMaxWidth},iw)/2)':-2:flags=lanczos`;
}

async function extractPosterWebp(source, destination, info, quality) {
    const seek = info.duration > 1 ? Math.min(info.duration * 0.15, 3) : 0;
    const framePath = path.join(tempDir, `${path.basename(destination, '.webp')}.png`);

    await run(ffmpegPath, [
        '-hide_banner', '-loglevel', 'error',
        '-i', source,
        '-ss', seek.toFixed(3),
        '-frames:v', '1',
        '-update', '1',
        '-y', framePath
    ]);

    await sharp(framePath)
        .resize({ width: 1280, withoutEnlargement: true })
        .webp({ quality })
        .toFile(destination);

    unlinkSync(framePath);
}

async function convertThumbnailGifs() {
    console.log('\n== GIF -> static WebP (card thumbnails) ==');
    for (const name of thumbnailGifs) {
        const source = path.join(gifDir, name);
        if (!existsSync(source)) {
            console.log(`  skip ${name} (missing)`);
            continue;
        }
        const destination = path.join(imageDir, `${path.basename(name, '.gif')}.webp`);
        const info = await probe(source);
        await extractPosterWebp(source, destination, info, 80);

        const before = sizeOf(source);
        const after = sizeOf(destination);
        report.push({ group: 'Thumbnail GIF', name, before, after });
        console.log(`  ${name}: ${mb(before).toFixed(2)} MB -> ${mb(after).toFixed(3)} MB`);
    }
}

async function convertClipGifs(withVp9) {
    console.log('\n== GIF -> MP4 + WebM + poster (project clips) ==');
    for (const name of clipGifs) {
        const source = path.join(gifDir, name);
        if (!existsSync(source)) {
            console.log(`  skip ${name} (missing)`);
            continue;
        }
        const stem = path.basename(name, '.gif');
        const mp4 = path.join(videoDir, `${stem}.mp4`);
        const webm = path.join(videoDir, `${stem}.webm`);
        const poster = path.join(imageDir, `${stem}-poster.webp`);

        const info = await probe(source);
        // These are screen recordings of gameplay, and many of the GIFs declare a nonsense
        // frame rate, so clamp rather than trust the source.
        const fps = Math.max(10, Math.min(Math.round(info.fps) || 15, 15));
        const filter = scaleFilter(fps);

        await run(ffmpegPath, [
            '-hide_banner', '-loglevel', 'error',
            '-i', source,
            '-vf', filter,
            '-c:v', 'libx264',
            '-crf', '28',
            '-preset', 'slow',
            '-profile:v', 'main',
            '-pix_fmt', 'yuv420p',
            '-movflags', '+faststart',
            '-an',
            '-y', mp4
        ]);

        if (withVp9) {
            await run(ffmpegPath, [
                '-hide_banner', '-loglevel', 'error',
                '-i', source,
                '-vf', filter,
                '-c:v', 'libvpx-vp9',
                '-crf', '36',
                '-b:v', '0',
                '-row-mt', '1',
                '-deadline', 'good',
                '-cpu-used', '2',
                '-an',
                '-y', webm
            ]);
        }

        await extractPosterWebp(source, poster, info, 75);

        const before = sizeOf(source);
        const after = sizeOf(mp4) + sizeOf(webm) + sizeOf(poster);
        report.push({ group: 'Clip GIF', name, before, after });
        console.log(
            `  ${name}: ${mb(before).toFixed(2)} MB -> ${mb(after).toFixed(3)} MB ` +
            `(mp4 ${mb(sizeOf(mp4)).toFixed(3)}, webm ${mb(sizeOf(webm)).toFixed(3)}, poster ${mb(sizeOf(poster)).toFixed(3)})`
        );
    }
}

async function convertRasterImages() {
    console.log('\n== PNG/JPG -> WebP ==');
    const { readdirSync } = await import('node:fs');
    const candidates = readdirSync(imageDir).filter((f) => /\.(png|jpe?g)$/i.test(f));

    for (const name of candidates) {
        const source = path.join(imageDir, name);
        if (unusedImages.includes(name)) {
            unlinkSync(source);
            report.push({ group: 'Unused image', name, before: 0, after: 0 });
            console.log(`  ${name}: deleted (referenced nowhere)`);
            continue;
        }

        const destination = path.join(imageDir, `${name.replace(/\.[^.]+$/, '')}.webp`);
        const maxWidth = imageMaxWidths[name] ?? defaultImageMaxWidth;
        const before = sizeOf(source);

        await sharp(source)
            .resize({ width: maxWidth, withoutEnlargement: true })
            .webp({ quality: 80 })
            .toFile(destination);

        const after = sizeOf(destination);
        report.push({ group: 'Raster image', name, before, after });
        console.log(`  ${name}: ${(before / 1024).toFixed(0)} KB -> ${(after / 1024).toFixed(0)} KB`);
    }
}

async function main() {
    if (!ffmpegPath || !existsSync(ffmpegPath)) {
        throw new Error('ffmpeg-static did not provide a binary. Run "npm install" in tools/media first.');
    }

    // "node convert-media.mjs clips" re-runs a single group after tweaking its settings.
    const requested = process.argv.slice(2).filter((a) => !a.startsWith('-'));
    const wants = (group) => requested.length === 0 || requested.includes(group);

    for (const dir of [imageDir, videoDir, tempDir]) {
        mkdirSync(dir, { recursive: true });
    }

    if (!(await hasEncoder('libx264'))) {
        throw new Error('The bundled ffmpeg has no libx264 encoder; cannot produce MP4 output.');
    }

    const withVp9 = await hasEncoder('libvpx-vp9');
    if (!withVp9) {
        console.warn('WARNING: bundled ffmpeg has no libvpx-vp9 encoder - emitting MP4 only.');
    }

    if (wants('thumbnails')) await convertThumbnailGifs();
    if (wants('clips')) await convertClipGifs(withVp9);
    if (wants('images')) await convertRasterImages();

    const totalBefore = report.reduce((sum, r) => sum + r.before, 0);
    const totalAfter = report.reduce((sum, r) => sum + r.after, 0);

    console.log('\n== Summary ==');
    console.log(`  before: ${mb(totalBefore).toFixed(2)} MB`);
    console.log(`  after:  ${mb(totalAfter).toFixed(2)} MB`);
    console.log(`  saved:  ${mb(totalBefore - totalAfter).toFixed(2)} MB ` +
        `(${(100 * (1 - totalAfter / Math.max(totalBefore, 1))).toFixed(1)}%)`);
    console.log('\nJSON_REPORT_START');
    console.log(JSON.stringify(report));
    console.log('JSON_REPORT_END');
}

main().catch((error) => {
    console.error(error.message);
    process.exit(1);
});