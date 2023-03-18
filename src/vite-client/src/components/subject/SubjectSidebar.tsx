import { Checkbox, Card, CardHeader, VStack, Heading } from "@chakra-ui/react";
import { v4 as uuidv4 } from "uuid";
import { useState, useEffect } from "react";

function SubjectSidebar() {
  const [subjects, setSubjects] = useState<[]>([]);

  useEffect(() => {
    getSubjects();
  }, []);

  function getSubjects() {
    fetch("/api/subjectHierarchy")
      .then((res) => res.json())
      .then((data) => {
        setSubjects(data);
      });
  }

  const HierarchyCheckbox = ({
    parent,
    padding,
  }: {
    parent: any;
    padding: number;
  }): JSX.Element => {
    return (
      <>
        <Checkbox m={1} pl={padding} defaultChecked>
          {parent["name"]}
        </Checkbox>
        {parent["subSubjects"] != undefined &&
          parent["subSubjects"].map((child: any, index: number) => {
            return (
              <HierarchyCheckbox
                key={uuidv4()}
                parent={child}
                padding={padding + 5}
              />
            );
          })}
      </>
    );
  };

  return (
    <Card variant={"outline"} w={"100%"} mt={"0 !important"}>
      <CardHeader>
        <Heading size={"md"}>Subject Hierarchy</Heading>
      </CardHeader>

      {subjects.map((subject: any, index: number) => {
        return (
          <Card key={index} ml={4} mb={3} border={"none"}>
            <HierarchyCheckbox key={uuidv4()} parent={subject} padding={0} />
          </Card>
        );
      })}
    </Card>
  );
}

export { SubjectSidebar };
