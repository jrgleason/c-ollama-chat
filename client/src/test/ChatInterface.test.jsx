import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import { ChatInterface } from '../components/ChatInterface'

describe('ChatInterface', () => {
  it('renders without crashing', () => {
    render(<ChatInterface />)
    // Basic smoke test - just ensure the component renders
    expect(document.body).toBeTruthy()
  })
})
