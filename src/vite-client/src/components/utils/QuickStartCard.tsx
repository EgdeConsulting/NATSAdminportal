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
  Icon,
  Link,
} from "@chakra-ui/react";
import { IconType } from "react-icons";
import { Link as RouteLink } from "react-router-dom";

function QuickStartCard(props: {
  header: string;
  description: string;
  image: IconType;
  route: string;
}) {
  return (
    <Card>
      <CardBody>
        <Icon as={props.image}></Icon>
        <Stack mt="6" spacing="3">
          <Heading size="md">{props.header}</Heading>
          <Text>{props.description}</Text>
        </Stack>
      </CardBody>
      <Divider />
      <CardFooter>
        <Link as={RouteLink} to={props.route}>
          <Button>{"Go to " + props.header}</Button>
        </Link>
      </CardFooter>
    </Card>
  );
}

export { QuickStartCard };
