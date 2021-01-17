import React from 'react'
import { Card, Button } from 'react-bootstrap'
import Image from 'react-bootstrap/Image'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'

export interface BlogCardProps {
  title: string
  description: string
  id: number
}

export const BlogCard: React.FC<BlogCardProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  return (
    <div style={{ marginTop: 5 }}>
      <Card className="flex-row flex-wrap">
        <Image src="https://picsum.photos/200/200" />
        <Card.Body>
          <Card.Title>{props.title}</Card.Title>
          <Card.Text>{props.description}</Card.Text>
          <Button onClick={() => push(`/blog/${props.id}`)} variant="primary">
            Read {props.id}
          </Button>
        </Card.Body>
      </Card>
    </div>
  )
}

export default BlogCard
