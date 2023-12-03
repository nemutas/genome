import * as THREE from 'three'
import vertexShader from './shader/screen.vs'
import fragmentShader from './shader/screen.fs'

export class Canvas {
  private renderer: THREE.WebGLRenderer
  private camera: THREE.PerspectiveCamera
  private scene: THREE.Scene
  private clock = new THREE.Clock()
  private screen: THREE.Mesh<THREE.PlaneGeometry, THREE.RawShaderMaterial>

  constructor(canvas: HTMLCanvasElement) {
    this.renderer = this.createRenderer(canvas)
    this.camera = this.createCamera()
    this.scene = this.createScene()
    this.addEvents()

    this.screen = this.createScreen()
    this.renderer.setAnimationLoop(this.anime)
  }

  private createRenderer(canvas: HTMLCanvasElement) {
    const renderer = new THREE.WebGLRenderer({ canvas, antialias: true, alpha: true })
    renderer.setSize(window.innerWidth, window.innerHeight)
    renderer.setPixelRatio(window.devicePixelRatio)
    return renderer
  }

  private createCamera() {
    const camera = new THREE.PerspectiveCamera(50, this.size.aspect, 0.01, 10)
    camera.position.z = 3.5
    return camera
  }

  private createScene() {
    const scene = new THREE.Scene()
    return scene
  }

  private get size() {
    const { width, height } = this.renderer.domElement
    return { width, height, aspect: width / height }
  }

  private createScreen() {
    const geometry = new THREE.PlaneGeometry(2, 2)
    const material = new THREE.RawShaderMaterial({
      uniforms: {
        uResolution: { value: [this.size.width, this.size.height] },
        uTime: { value: 0 },
      },
      vertexShader,
      fragmentShader,
    })
    const mesh = new THREE.Mesh(geometry, material)
    this.scene.add(mesh)

    return mesh
  }

  private addEvents() {
    window.addEventListener('resize', () => {
      const { innerWidth: width, innerHeight: height } = window
      this.renderer.setSize(width, height)
      this.camera.aspect = width / height
      this.camera.updateProjectionMatrix()
      //
      this.screen.material.uniforms.uResolution.value = [this.size.width, this.size.height]
    })
  }

  private anime = () => {
    this.screen.material.uniforms.uTime.value += this.clock.getDelta()

    this.renderer.render(this.scene, this.camera)
  }

  dispose() {}
}
