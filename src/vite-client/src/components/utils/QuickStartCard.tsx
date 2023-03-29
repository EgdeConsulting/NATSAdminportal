import {
  Card,
  Image,
  Button,
  ButtonGroup,
  CardBody,
  CardFooter,
  Divider,
  Heading,
  Stack,
  Text,
  Link,
  CardHeader,
  Container,
} from "@chakra-ui/react";
import { Link as RouteLink } from "react-router-dom";

function QuickStartCard(props: {
  header: string;
  description: string;
  image: JSX.Element;
  route: string;
}) {
  return (
    <Card width={"50%"} align={"center"}>
      <CardBody>
        <Container maxW={"500px"} centerContent={true}>
          {props.image}
          <Stack align={"center"} mt="6" spacing="3">
            <Heading size="md">{props.header}</Heading>
            <Text align={"center"}>{props.description}</Text>
          </Stack>
        </Container>
      </CardBody>
      <Divider w={"93%"} />
      <CardFooter>
        <Link as={RouteLink} to={props.route}>
          <Button>{"Go to " + props.header}</Button>
        </Link>
      </CardFooter>
    </Card>
  );
}

export { QuickStartCard };
