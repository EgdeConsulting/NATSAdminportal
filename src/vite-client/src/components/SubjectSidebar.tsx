import {
  Checkbox,
  CheckboxGroup,
  Drawer,
  Flex,
  Spacer,
  Box,
  Divider,
  Card,
  CardHeader,
  CardBody,
  Stack,
  VStack,
  Heading,
} from "@chakra-ui/react";
import { useState } from "react";

function SubjectSidebar() {
  const [checkedItems, setCheckedItems] = useState([false, false]);

  const allChecked = checkedItems.every(Boolean);
  const isIndeterminate = checkedItems.some(Boolean) && !allChecked;

  return (
    <div>
      <VStack margin={2} w={"100%"} h={"100%"}>
        <Card variant={"filled"} h={"100%"} w={"100%"}>
          <CardHeader>
            <Heading size={"md"}>Subject Hierarchy</Heading>
          </CardHeader>
          <CardBody justifyContent={"center"}>
            <Checkbox
              isChecked={allChecked}
              isIndeterminate={isIndeterminate}
              onChange={(e) =>
                setCheckedItems([e.target.checked, e.target.checked])
              }
            >
              Subject
            </Checkbox>
            <Stack pl={6} mt={1} spacing={1}>
              <Checkbox
                isChecked={checkedItems[0]}
                onChange={(e) =>
                  setCheckedItems([e.target.checked, checkedItems[1]])
                }
              >
                A
              </Checkbox>
              <Checkbox
                isChecked={checkedItems[1]}
                onChange={(e) =>
                  setCheckedItems([checkedItems[0], e.target.checked])
                }
              >
                B
              </Checkbox>
            </Stack>
          </CardBody>
        </Card>
      </VStack>
    </div>
  );
}

export { SubjectSidebar };
